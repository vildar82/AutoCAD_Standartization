using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace AutoCAD_Standartization
{
    public class TextStyleManager
    {
        Database targetDB = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

        //Типы сканируемых объектов
        Type[] types = new Type[] { typeof(DBText), typeof(MText), typeof(DimStyleTableRecord), 
                                    typeof(MLeaderStyle), typeof(TableStyle), typeof(Table),
                                    typeof(AttributeDefinition),typeof(AttributeReference)};


        public void TransferTextStyles()
        {
            LayerManager lm = new LayerManager();
            Database dbStandart = new Database(false, true);
            dbStandart.ReadDwgFile(lm.PathToStadartFile, FileShare.Read, true, "");
            //Список стилей в стандарте
            List<TextStyleTableRecord> listStandartTextStyles = GetStyles(dbStandart);
            //стиль для установки в текущий   
            TextStyleTableRecord pikStyle = listStandartTextStyles.Select(x => x).Where(x => x.Name.Equals("PIK")).ToList()[0];
            //Копируем основной стиль из стандарта
            CopyEtalonStyle(listStandartTextStyles);
            //Список стилей в документе
            List<TextStyleTableRecord> listActiveDocTextStyles = GetStyles(targetDB);
            //Получаем основной стиль в документе
            TextStyleTableRecord pikStyleActive = listActiveDocTextStyles.Select(x => x).Where(x => x.Name.Equals("PIK")).ToList()[0];
            //Устанавливаем текущий стиль   
            Application.SetSystemVariable("TEXTSTYLE", pikStyleActive.Name);
            List<string> exceptLayers = new List<string>();
            //Вычитание слоев для их удаления
            exceptLayers = listActiveDocTextStyles.Select(x => x.Name).Except((listStandartTextStyles).Select(y => y.Name)).ToList();

            List<string> intersectLayers = new List<string>();
            //Пересечение слоев для установки свойств стандарта 
            intersectLayers = listActiveDocTextStyles.Select(x => x.Name).Intersect((listStandartTextStyles).Select(y => y.Name)).ToList();
            Dictionary<string, ObjectId[]> dicDeleteTextStyles = GetDependentPrimitives(listActiveDocTextStyles.Select(x => x.Name).ToArray());
            //foreach (TextStyleTableRecord textRecord in listStandartTextStyles)
            //{
            //    //Установка свойств стандарта
            //    SetSettings(textRecord.Name, textRecord);
            //}
            //Замена и удаление стилей нестандартных
            Union(dicDeleteTextStyles, pikStyleActive);

        }

        private void CopyEtalonStyle(List<TextStyleTableRecord> pikStyle)
        {
            ObjectIdCollection col = new ObjectIdCollection();
            IdMapping acIdMap = new IdMapping();
            foreach (var item in pikStyle)
            {
                col.Add(item.ObjectId);
            }
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)t.GetObject(targetDB.TextStyleTableId, OpenMode.ForRead);
                targetDB.WblockCloneObjects(col, tst.Id, acIdMap, DuplicateRecordCloning.Replace, false);
                t.Commit();
            }
        }
        private Dictionary<string, ObjectId[]> GetDependentPrimitives(string[] styleNames)
        {
            //Если стили, подлежащие уничтожению на найдены - возвращаем null
            if (styleNames.Length == 0)
                return null;

            Dictionary<string, List<ObjectId>> result = new Dictionary<string, List<ObjectId>>();

         try
         {
            foreach (string item in styleNames) result.Add(item.Trim().ToUpper(), new List<ObjectId>());
         }
         catch (Exception ex)
         {            
         }            


            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                try
                {
                    GetPrimitives().Where(n => types.Contains(t.GetObject(n, OpenMode.ForRead).GetType())).All(n => { Fill(n, result); return true; });
                }
                catch { }
                Dictionary<string, ObjectId[]> dict = new Dictionary<string, ObjectId[]>();
                foreach (var item in result)
                    dict.Add(item.Key.ToUpper(), item.Value.ToArray());
                return dict;
            }
        }


        private List<TextStyleTableRecord> GetStyles(Database db)
        {
            List<TextStyleTableRecord> allTextStyles = new List<TextStyleTableRecord>();
            List<TextStyleTableRecord> x = new List<TextStyleTableRecord>();
            using (Transaction t = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)t.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                allTextStyles = tst.Cast<ObjectId>().Select(n => (TextStyleTableRecord)t.GetObject(n, OpenMode.ForRead)).ToList();  
            }
            foreach (TextStyleTableRecord item in allTextStyles)
            {
                if (item.Name.Contains('|')) continue;
                x.Add(item);
            }
            return x;
        }

        private ObjectId GetStyleByName(string textStyleName)
        {
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)t.GetObject(targetDB.TextStyleTableId, OpenMode.ForRead);
                return (ObjectId)t.GetObject(tst[textStyleName], OpenMode.ForRead).ObjectId;
            }
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
                    if (id.IsErased) continue;         //фильтр удаленных
                    if (!id.IsValid) continue;
                    objCollection.Add(id);

                }
            }
            return objCollection;
        }
        public bool Exists(string styleName)
        {
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = (TextStyleTable)t.GetObject(targetDB.TextStyleTableId, OpenMode.ForRead);
                return tst.Has(styleName);
            }
        }


        private void Fill(ObjectId id, Dictionary<string, List<ObjectId>> dict)
        {
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                DBObject item = t.GetObject(id, OpenMode.ForRead);
                //Если это размерный стиль
                if (item is DimStyleTableRecord)
                {
                    DimStyleTableRecord x = (DimStyleTableRecord)item;
                    //try
                    //{

                        string styleName = ((TextStyleTableRecord)t.GetObject(x.Dimtxsty, OpenMode.ForRead)).Name.Trim().ToUpper();
                        if (dict.Keys.Contains(styleName))
                            dict[styleName].Add(item.ObjectId);
                    //}
                    //catch { } 
            return;
                   
                }
                //Если это стиль мультивыноски
                else if (item is MLeaderStyle)
                {
                    MLeaderStyle x = (MLeaderStyle)item;
                    string styleName = ((TextStyleTableRecord)t.GetObject(x.TextStyleId, OpenMode.ForRead)).Name.Trim().ToUpper();
                    if (dict.Keys.Contains(styleName))
                        dict[styleName].Add(item.ObjectId);
                    return;
                }
                //Если это стиль таблиц
                else if (item is TableStyle)
                {
                    TableStyle ts = (TableStyle)item;
                    //По умолчанию каждый табличный стиль определяет в своём составе три стиля ячеек:
                    //_TITLE, _HEADER и _DATA. В дополнение к этим стилям, пользователь может определить
                    //свои. Каждый из стилей ячеек определяет в своём составе (помимо всего прочего) то, 
                    //какой ТЕКСТОВЫЙ СТИЛЬ должен использоваться в ячейках, использующих этот СТИЛЬ
                    //ЯЧЕЕК.

                    //Проверяем каждый имеющийся стиль ячеек на предмет того, какой текстовый стиль
                    //обозначен в их настройках к использованию для ячеек таблицы
                    foreach (string cellStyle in ts.CellStyles)
                    {
                        string styleName = GetStyle(ts.TextStyle(cellStyle)).Name.Trim().ToUpper();
                        if (dict.Keys.Contains(styleName) && !dict[styleName].Contains(item.ObjectId))
                        {
                            dict[styleName].Add(item.ObjectId);
                            return;
                        }
                    }
                }
                //Если это однострочный текст
                else if (item is DBText)
                {
                    DBText txt = (DBText)item;
                    string styleName = ((TextStyleTableRecord)t.GetObject(txt.TextStyleId, OpenMode.ForRead)).Name.Trim().ToUpper();
                    if (dict.Keys.Contains(styleName))
                        dict[styleName].Add(item.ObjectId);
                    return;
                }
                //Если это многострочный текст
                else if (item is MText)
                {
                    MText txt = (MText)item;
                    string styleName = ((TextStyleTableRecord)t.GetObject(txt.TextStyleId, OpenMode.ForRead)).Name.Trim().ToUpper();
                    if (dict.Keys.Contains(styleName))
                        dict[styleName].Add(item.ObjectId);
                    return;
                }
                else if (item is Table)
                {
                    Table table = (Table)item;
                    //Сначала проверяю, какие стили текста назначены столбцам. ПОЧЕМУ-ТО ВСЕГДА 
                    //ПОЛУЧАЮ "", НЕ СМОТРЯ НА ТО, ЧТО СТОЛБЦУ МОЖЕТ БЫТЬ НАЗНАЧЕН КОНКТЕТНЫЙ СТИЛЬ ТЕКСТА
                    for (int i = 0; i < table.NumColumns; i++)
                    {
                        string styleName = table.GetCellStyle(-1, i).ToUpper().Trim().ToUpper();
                        if (dict.Keys.Contains(styleName) && !dict[styleName].Contains(item.ObjectId))
                        {
                            dict[styleName].Add(item.ObjectId);
                            return;
                        }
                    }

                    //Теперь проверяю, какие стили текста назначены строкам. ПОЧЕМУ-ТО ВСЕГДА 
                    //ПОЛУЧАЮ "", НЕ СМОТРЯ НА ТО, ЧТО СТРОКЕ МОЖЕТ БЫТЬ НАЗНАЧЕН КОНКТЕТНЫЙ СТИЛЬ ТЕКСТА 
                    for (int i = 0; i < table.NumRows; i++)
                    {
                        string styleName = table.GetCellStyle(i, -1).ToUpper().Trim().ToUpper();
                        if (dict.Keys.Contains(styleName) && !dict[styleName].Contains(item.ObjectId))
                        {
                            dict[styleName].Add(item.ObjectId);
                            return;
                        }
                    }
                    //Анализируем каждую ячейку таблицы
                    //Цикл по столбцам
                    for (int i = 0; i < table.NumColumns; i++)
                    {
                        //Цикл по строкам
                        for (int k = 0; k < table.NumRows; k++)
                        {
                            //Анализируем конкретную ячейку таблицы
                            string styleName = table.GetCellStyle(k, i).ToUpper().Trim().ToUpper();
                            if (dict.Keys.Contains(styleName) && !dict[styleName].Contains(item.ObjectId))
                            {
                                dict[styleName].Add(item.ObjectId);
                                return;
                            }
                        }

                    }
                }
                else
                    throw new NotImplementedException();
            }
        }

        public TextStyleTableRecord GetStyle(ObjectId styleId)
        {
            using (Transaction t = targetDB.TransactionManager.StartTransaction())
            {
                return (TextStyleTableRecord)t.GetObject(styleId, OpenMode.ForRead);
            }
        }

        public void SetSettings(string styleName, TextStyleTableRecord stdStyle)
        {
            if (Exists(styleName))
                using (Transaction t = targetDB.TransactionManager.StartTransaction())
                {
                    TextStyleTable tst = (TextStyleTable)t.GetObject(targetDB.TextStyleTableId,
                          OpenMode.ForRead);
                    TextStyleTableRecord ts = (TextStyleTableRecord)t.GetObject(tst[styleName],
                           OpenMode.ForWrite);

                    ts.FileName = stdStyle.FileName;
                    ts.Annotative = stdStyle.Annotative;
                    ts.ObliquingAngle = stdStyle.ObliquingAngle;
                    // ts.SetPaperOrientation(stdStyle.PaperOrientation);
                    ts.BigFontFileName = stdStyle.BigFontFileName;
                    ts.TextSize = stdStyle.TextSize;
                    ts.XScale = stdStyle.XScale;
                    ts.IsVertical = stdStyle.IsVertical;
                    t.Commit();
                }
        }

        public void Union(Dictionary<string, ObjectId[]> dicDeleteTextStyles, TextStyleTableRecord resultStyleName)
        {

            using (Transaction tr = targetDB.TransactionManager.StartTransaction())
            {
                foreach (var style in dicDeleteTextStyles)
                {
                    AttributeDefinition[] attdefs = style.Value.Where(n => tr.GetObject(n,
                        OpenMode.ForRead) is AttributeDefinition).Select(n =>
                            (AttributeDefinition)tr.GetObject(n, OpenMode.ForWrite)).ToArray();

                    foreach (AttributeDefinition attdef in attdefs)
                    {
                        attdef.TextStyleId = resultStyleName.ObjectId;
                        // attdef.UpdateMTextAttributeDefinition();

                    }

                    foreach (var _x in style.Value)
                    {
                        DBObject item = tr.GetObject(_x, OpenMode.ForWrite);
                        //Если это размерный стиль
                        if (item is DimStyleTableRecord)
                        {
                            DimStyleTableRecord dstr = (DimStyleTableRecord)item;
                            dstr.Dimtxsty = resultStyleName.ObjectId;
                        }
                        //Если это стиль мультивыноски
                        else if (item is MLeaderStyle)
                        {
                            MLeaderStyle mls = (MLeaderStyle)item;
                            mls.TextStyleId = resultStyleName.ObjectId;
                        }
                        //Если это табличный стиль
                        else if (item is TableStyle)
                        {
                            TableStyle ts = (TableStyle)item;

                            //В цикле проверяем текстовый стиль каждой ячейки таблицы
                            foreach (string cellStyle in ts.CellStyles)
                            {
                                if (ts.TextStyle(cellStyle).Database == null) continue;
                                if (dicDeleteTextStyles.Keys.Contains(GetStyle(ts.TextStyle(cellStyle))
                                     .Name.Trim().ToUpper()))
                                    ts.SetTextStyle(resultStyleName.ObjectId, cellStyle);
                            }
                        }
                        //Если это однострочный текст, не являющийся при этом атрибутом 
                        //определения блока
                        else if ((item is DBText) && !(item is AttributeDefinition))
                        {
                            DBText txt = (DBText)item;
                            
                            txt.TextStyleId = resultStyleName.ObjectId;
                            txt.Oblique = 0;

                            //if (txt is AttributeReference)
                            //   ((AttributeReference)txt).UpdateMTextAttribute();

                        }
                        //Если это многострочный текст
                        else if (item is MText)
                        {
                            MText txt = (MText)item;
                            txt.TextStyleId = resultStyleName.ObjectId;
                        }
                        //Если это таблица
                        else if (item is Table)
                        {
                            Table table = (Table)item;
                            string[] styleNames = dicDeleteTextStyles.Keys.ToArray();
                            //Сначала проверяю, какие стили текста назначены столбцам
                            for (int i = 0; i < table.NumColumns; i++)
                                if (styleNames.Contains(table.GetCellStyle(-1, i).ToUpper().Trim()))
                                {
                                    try
                                    {
                                        table.SetCellStyle(-1, i, resultStyleName.Name);
                                    }
                                    catch { }

                                    
                                }
                            //Теперь проверяю, какие стили текста назначены строкам
                            for (int i = 0; i < table.NumRows; i++)
                                if (styleNames.Contains(table.GetCellStyle(i, -1).ToUpper().Trim()))
                                {
                                    
                                    try
                                    {
                                        table.SetCellStyle(i, -1, resultStyleName.Name);
                                    }
                                    catch { }
                                }
                            //Анализируем каждую ячейку таблицы
                            //Цикл по столбцам
                            for (int i = 0; i < table.NumColumns; i++)
                            {
                                //Цикл по строкам
                                for (int k = 0; k < table.NumRows; k++)
                                {
                                    //Анализируем конкретную ячейку таблицы
                                    if (styleNames.Contains(table.GetCellStyle(k, i).ToUpper().Trim()))
                                    {
                                       
                                        try
                                        {
                                            table.SetCellStyle(k, i, resultStyleName.Name);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }

                    if (CanBeRemoved(style.Key))
                    {
                        TextStyleTableRecord tsr = (TextStyleTableRecord)tr.GetObject(GetStyleByName(style.Key), OpenMode.ForWrite, true);
                        tsr.Erase();
                    }

                }
                tr.Commit();
            }
        }

        public bool CanBeRemoved(string styleName)
        {

            if ((styleName.ToUpper() == "STANDARD") || (styleName.ToUpper() == "ANNOTATIVE") || (styleName.ToUpper() == "PIK"))
                return false;
            return true;
            //ObjectIdCollection c = new ObjectIdCollection(new ObjectId[] { 
            //    GetStyleByName(styleName) });
            //targetDB.Purge(c);
            //return c.Count != 0;
        }
    }
}
