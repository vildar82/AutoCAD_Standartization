using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoCAD_Standartization
{
    public class DimStyleManager
    {
        Database targetDB = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

        //Типы сканируемых объектов
        Type[] types = new Type[] { typeof(Dimension) };


        public void TransferDimStyles()
        {
            LayerManager lm = new LayerManager();
            Database dbStandart = new Database(false, true);
            dbStandart.ReadDwgFile(lm.PathToStadartFile, FileShare.Read, true, "");
            List<DimStyleTableRecord> standartDimStyless = GetDimStyles(dbStandart);

            ObjectIdCollection col = GetStandartDimStyles(standartDimStyless, true);
            CopyEtalonStyle(col);
            List<DimStyleTableRecord> docDimStyles = GetDimStyles(targetDB);
            DimStyleTableRecord pikStyleActive = docDimStyles.Select(x => x).Where(x => x.Name.ToUpper().Equals("PIK")).ToList()[0];
            DimStyleTableRecord pikStyleDiametricDimension = docDimStyles.Select(x => x).Where(x => x.Name.ToUpper().Equals("PIK$2")).ToList()[0];
            DimStyleTableRecord pikStyleRadialDimension = docDimStyles.Select(x => x).Where(x => x.Name.ToUpper().Equals("PIK$3")).ToList()[0];
            DimStyleTableRecord pikStyleLineAngularDimension = docDimStyles.Select(x => x).Where(x => x.Name.ToUpper().Equals("PIK$4")).ToList()[0];
            DimStyleTableRecord[] masDimStyles = new DimStyleTableRecord[] { pikStyleActive, pikStyleDiametricDimension, pikStyleRadialDimension, pikStyleLineAngularDimension };
            targetDB.Dimstyle = pikStyleActive.ObjectId;
            Dictionary<string, ObjectId[]> dicDeleteDimStyles = GetDependentPrimitives(docDimStyles.Select(x => x.Name).ToArray());
            Union(dicDeleteDimStyles, masDimStyles);
        }

        public List<ObjectId> GetPrimitives()
        {
            List<ObjectId> objCollection = new List<ObjectId>();
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                for (long i = targetDB.BlockTableId.Handle.Value; i < targetDB.Handseed.Value; i++)
                {
                    ObjectId id = ObjectId.Null;
                    Handle h = new Handle(i);
                    targetDB.TryGetObjectId(h, out id);
                    if (!id.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Dimension)))) continue;
                    if (id.IsErased) continue;
                    if (!id.IsValid) continue;

                    var op = id.GetType();
                    var k = id.ObjectClass;
                    objCollection.Add(id);

                }
            }
            return objCollection;
        }
        private Dictionary<string, ObjectId[]> GetDependentPrimitives(string[] styleNames)
        {
            //Если стили, подлежащие уничтожению на найдены - возвращаем null
            if (styleNames.Length == 0)
                return null;

            Dictionary<string, List<ObjectId>> result = new Dictionary<string, List<ObjectId>>();

            foreach (string item in styleNames) result.Add(item.Trim().ToUpper(), new List<ObjectId>());


            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                try
                {
                    GetPrimitives().Where(n => (t.GetObject(n, OpenMode.ForRead).ObjectId.ObjectClass.DxfName.Contains("DIMENSION"))).All(n => { Fill(n, result); return true; });
                }
                catch { }
                Dictionary<string, ObjectId[]> dict = new Dictionary<string, ObjectId[]>();
                foreach (var item in result)
                    dict.Add(item.Key.ToUpper(), item.Value.ToArray());
                return dict;
            }
        }
        ObjectIdCollection GetStandartDimStyles(List<DimStyleTableRecord> standartDimStyless, bool isFirstCopy)
        {
            ObjectIdCollection col = new ObjectIdCollection();
            foreach (DimStyleTableRecord style in standartDimStyless)
            {
                col.Add(style.ObjectId);
            }

            return col;
        }

        private List<DimStyleTableRecord> GetDimStyles(Database db)
        {
            List<DimStyleTableRecord> alldimStyles = new List<DimStyleTableRecord>();
            List<DimStyleTableRecord> dimStyles = new List<DimStyleTableRecord>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DimStyleTable tblStyle = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                alldimStyles = tblStyle.Cast<ObjectId>().Select(n => (DimStyleTableRecord)tr.GetObject(n, OpenMode.ForRead)).ToList();

            }
            foreach (DimStyleTableRecord item in alldimStyles)
            {
                if (item.Name.Contains('|')) continue;
                dimStyles.Add(item);
            }
            return dimStyles;
        }

        private void CopyEtalonStyle(ObjectIdCollection col)
        {
            IdMapping acIdMap = new IdMapping();
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                DimStyleTable tst = (DimStyleTable)t.GetObject(targetDB.DimStyleTableId, OpenMode.ForRead);
                // ObjectIdCollection col = new ObjectIdCollection();
                // col.Add(pikStyle.ObjectId);

                targetDB.WblockCloneObjects(col, tst.Id, acIdMap, DuplicateRecordCloning.Replace, false);
                t.Commit();
            }
        }

        private void Fill(ObjectId id, Dictionary<string, List<ObjectId>> dict)
        {
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                DBObject item = t.GetObject(id, OpenMode.ForRead);
                //Если это размерный стиль
                if (item.ObjectId.ObjectClass.IsDerivedFrom(RXClass.GetClass(typeof(Dimension))))
                {
                    Dimension y = (Dimension)item;
                    string styleName = y.DimensionStyleName;
                    if (dict.Keys.Contains(styleName.ToUpper()))
                        dict[styleName.ToUpper()].Add(item.ObjectId);
                }

                else
                    throw new NotImplementedException();
            }
        }

        public void Union(Dictionary<string, ObjectId[]> dicDeleteDimStyles, DimStyleTableRecord[] resultStyleName)
        {

            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                DimStyleTable tblStyle = (DimStyleTable)t.GetObject(targetDB.DimStyleTableId, OpenMode.ForWrite);
                foreach (var _style in dicDeleteDimStyles)
                {

                    foreach (var _x in _style.Value)
                    {
                        DBObject item = t.GetObject(_x, OpenMode.ForWrite);
                        Dimension y = (Dimension)item;
                        if (y.Annotative != AnnotativeStates.True)
                            SetStyleAnnotative(resultStyleName, item, y);
                        SetStyleByType(resultStyleName, item, y);
                    }


                    if (!CanBeRemoved(_style.Key)) continue;

                    List<DimStyleTableRecord> dimStyles = tblStyle.Cast<ObjectId>().Select(n => (DimStyleTableRecord)t.GetObject(n, OpenMode.ForWrite, true)).ToList();
                    DimStyleTableRecord styleDel = dimStyles.Select(x => x).Where(x => x.Name.ToUpper().Equals(_style.Key)).ToList()[0];
                    styleDel.Erase();

                }
                t.Commit();
            }
        }

        private void SetStyleAnnotative(DimStyleTableRecord[] resultStyleName, DBObject item, Dimension y)
        {
            AnnotationScale mainScale = new AnnotationScale();
            ObjectContextManager ocm = targetDB.ObjectContextManager;
            double scale = 2.5 / (y.Dimscale * y.Dimtxt);
            double difference = 200;
            if (ocm != null)
            {
                ObjectContextCollection occ = ocm.GetContextCollection("ACDB_ANNOTATIONSCALES");
                ObjectContext currentContext = occ.CurrentContext;
                foreach (ObjectContext context in occ)
                {

                    double currentDifference = 200;
                    AnnotationScale annotScale = (AnnotationScale)context;
                    if (annotScale.Scale == scale)
                    {
                        mainScale = annotScale;
                        break;
                    }
                    else
                    {
                        currentDifference = Math.Abs(scale - annotScale.Scale);
                        if (currentDifference < difference)
                        {
                            difference = currentDifference;
                            mainScale = annotScale;
                        }
                    }
                }
                SetStyleByType(resultStyleName, item, y);
                if (y.HasContext(currentContext))
                    y.RemoveContext(currentContext);
                y.AddContext(mainScale);
                y.Dimtxt = 2.5;
            }
        }

        private static void SetStyleByType(DimStyleTableRecord[] resultStyleName, DBObject item, Dimension y)
        {
            if (item is LineAngularDimension2)
            {
                Dimension y2 = (LineAngularDimension2)item;
                y2.DimensionStyleName = resultStyleName[1].Name;
            }
            else if (item is DiametricDimension)
            {
                Dimension y2 = (DiametricDimension)item;
                y2.DimensionStyleName = resultStyleName[2].Name;
            }
            else if (item is RadialDimension)
            {
                Dimension y2 = (RadialDimension)item;
                y2.DimensionStyleName = resultStyleName[3].Name;
            }
            else y.DimensionStyleName = resultStyleName[0].Name;
        }

        public bool CanBeRemoved(string styleName)
        {
            if ((styleName.ToUpper() == "STANDARD") || (styleName.ToUpper() == "ANNOTATIVE") ||
                (styleName.ToUpper() == "PIK") || (styleName.ToUpper() == "АННОТАТИВНЫЙ") || (styleName.ToUpper().Contains("PIK")))
                return false;
            return true;
            //ObjectIdCollection c = new ObjectIdCollection(new ObjectId[] { 
            //    GetStyleByName(styleName) });
            //targetDB.Purge(c);
            //return c.Count != 0;
        }
    }
}
