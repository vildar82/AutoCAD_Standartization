using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
//using Autodesk.AutoCAD.Interop;
using AutoCadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AutoCAD_Standartization
{
   public class LayerManager
   {
      private static string codeGroup = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup; // GetMyCodeGroup();
      public string PathToStadartFile = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder,
                                        string.Format("Standart\\{0}\\{0}.dws", codeGroup)); //@"C:\Autodesk\AutoCAD\Pik\Settings\Standart" + "\\" + codeGroup + "\\" + codeGroup + ".dws";
      public string PathToXMLFile = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder,
                                    string.Format("Standartization\\{0}\\DataLayers.xml", codeGroup));//  @"Z:\AutoCAD_server\ShareSettings\Standartization" + "\\" + codeGroup + "\\" + "DataLayers.xml";
      public List<LayerTableRecord> GetListLayers(Database db, bool isStandart)       //Список всех слоев в документе
      {
         List<LayerTableRecord> listLayers = new List<LayerTableRecord>();
         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            if (isStandart)
            {
               MainForm.dicLineTypes = new Dictionary<long, string>();
               LinetypeTable acLinTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite) as LinetypeTable;
               foreach (var item in acLinTbl)
               {
                  LinetypeTableRecord btr = (LinetypeTableRecord)tr.GetObject(item, OpenMode.ForRead);
                  MainForm.dicLineTypes.Add(btr.Id.OldId, btr.Name);
               }
            }

            var layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
            foreach (var layer in layerTable)
            {
               LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layer, OpenMode.ForWrite, true);
               ltr.IsLocked = false;
               listLayers.Add(ltr);
            }
            tr.Commit();
         }
         return listLayers;
      }

      public static void DeleteLayer(string layerName, Database db)               //Удаление слоя
      {
         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            LinetypeTable acLinTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite) as LinetypeTable;
            var layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
            foreach (var layer in layerTable)
            {
               LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layer, OpenMode.ForWrite, true);
               if (ltr.Name.Equals("0")) db.Clayer = ltr.Id;                                               //обязательная активация в текущий слой 0
               if (!ltr.Name.Equals(layerName)) continue;
               try
               {
                  ltr.IsLocked = false;
                  ltr.Erase();
                  break;
               }
               catch { }
            }
            tr.Commit();
         }
      }


      public ObjectIdCollection GetEntitiesOnLayer(string layerName)
      {
         Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
         Editor ed = doc.Editor;
         TypedValue[] tvs = new TypedValue[1] { new TypedValue((int)DxfCode.LayerName, layerName) };
         SelectionFilter sf = new SelectionFilter(tvs);
         PromptSelectionResult psr = ed.SelectAll(sf);
         if (psr.Status == PromptStatus.OK)
            return new ObjectIdCollection(psr.Value.GetObjectIds());
         else return new ObjectIdCollection();
      }

      public bool TransferLayer(Dictionary<string, string> dicLayers)
      {
         Document thisDrawing = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
         Database db = thisDrawing.Database;
         LayerTableRecord stdLayer = null;
         var stdDatabase = new Database(false, true);
         stdDatabase.ReadDwgFile(PathToStadartFile, FileShare.Read, true, "");
         List<LayerTableRecord> listStd = GetListLayers(stdDatabase, true);
         using (DocumentLock docLock = thisDrawing.LockDocument())
         {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
               LayerTable layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForWrite, true) as LayerTable;
               foreach (var dicLayer in dicLayers)
               {
                  string activeLayerName = dicLayer.Key;
                  string stdLayerName = dicLayer.Value;
                  foreach (LayerTableRecord layer in listStd)
                  {
                     if (layer.Name != stdLayerName) continue;
                     stdLayer = layer;
                  }
                  var op = MainForm.dicLineTypes;
                  SetStandartToLayer(db, stdLayer, tr, activeLayerName, stdLayerName, layerTable);
               }
               tr.Commit();
            }
         }
         return true;
      }

      private static void SetStandartToLayer(Database db, LayerTableRecord stdLayer, Transaction tr, string activeLayerName, string stdLayerName, LayerTable layerTable)
      {
         //  var stdDatabase = new Database(false, true);
         // stdDatabase.ReadDwgFile(@"Z:\AutoCAD_server\ShareSettings\КР-сб\КР-сб.dws", FileShare.Read, true, "");
         foreach (var layer in layerTable)
         {
            LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(layer, OpenMode.ForWrite);
            if (!ltr.Name.Equals(activeLayerName)) continue;
            ltr.IsLocked = false;
            ltr.UpgradeOpen();
            if (activeLayerName == stdLayerName)
            {
               SetLayerProperties(stdLayer, ltr, db, tr);
            }
            else if (!layerTable.Has(stdLayerName))
            {
               ltr.Name = stdLayerName;
               SetLayerProperties(stdLayer, ltr, db, tr);
            }
            else
            {
               bool isLock = false;
               foreach (var layerStd in layerTable)
               {
                  LayerTableRecord ltrstd = (LayerTableRecord)tr.GetObject(layerStd, OpenMode.ForWrite);
                  if (!ltrstd.Name.Equals(stdLayerName)) continue;
                  isLock = ltrstd.IsLocked;
                  ltrstd.IsLocked = false;
                  SetLayerProperties(stdLayer, ltrstd, db, tr);
                  break;
               }
               SetLayerToEntity(db, activeLayerName, stdLayerName);
               ltr.IsLocked = isLock;
               DeleteLayer(activeLayerName, db);
            }
         }
      }

      private static void SetLayerProperties(LayerTableRecord stdLayer, LayerTableRecord ltr, Database db, Transaction tr)
      {
         ltr.Color = stdLayer.Color;
         ltr.LineWeight = stdLayer.LineWeight;
         ObjectIdCollection acObjIdColl = new ObjectIdCollection();
         acObjIdColl.Add(stdLayer.LinetypeObjectId);
         IdMapping acIdMap = new IdMapping();

         LinetypeTable acLinTbl; acLinTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForWrite) as LinetypeTable;
         db.WblockCloneObjects(acObjIdColl, acLinTbl.Id, acIdMap,
                                    DuplicateRecordCloning.Ignore, false);
         LinetypeTableRecord currentLineType = GetLineType(tr, stdLayer.LinetypeObjectId, acLinTbl);

         ltr.LinetypeObjectId = currentLineType.Id;
         ltr.IsPlottable = stdLayer.IsPlottable;
         ltr.Description = stdLayer.Description;
      }

      private static LinetypeTableRecord GetLineType(Transaction tr, ObjectId idLineStd, LinetypeTable acLinTbl)
      {
         LinetypeTableRecord currentLineType = null;
         string nameLineType = "";
         foreach (var item in MainForm.dicLineTypes)
         {
            if (idLineStd.OldId != item.Key) continue;
            nameLineType = item.Value;
            break;
         }

         foreach (var item in acLinTbl)
         {
            LinetypeTableRecord lineTbr = (LinetypeTableRecord)tr.GetObject(item, OpenMode.ForRead);

            if (lineTbr.Name.ToUpper() == nameLineType.ToUpper())
            {
               currentLineType = lineTbr;
               return currentLineType;

            }
         }
         return currentLineType;
      }

      public static void SetLayerToEntity(Database db, string activeLayerName, string stdLayerName)
      {
         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            foreach (var btrId in bt)
            {
               var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);

               if ((btr.IsFromExternalReference) || (btr.IsDependent && btr.Name.Contains("|")))
                  continue;
               foreach (var entId in btr)
               {
                  Entity ent = null;
                  try
                  {
                     ent = (Entity)tr.GetObject(entId, OpenMode.ForWrite);
                  }
                  catch { continue; }
                  if (ent.Layer.Equals(activeLayerName))
                  {
                     ent.UpgradeOpen();
                     ent.Layer = stdLayerName;
                  }
               }
            }
            tr.Commit();
         }
      }

      public void SaveDataLayers(Dictionary<string, string> dicLayers)
      {
         SerializerXml serialize = new SerializerXml();
         foreach (var item in dicLayers)
         {
            TranferLayer dataTransferLayer = new TranferLayer();
            dataTransferLayer.AuthorMark = Environment.UserName;
            dataTransferLayer.LayerDocument = item.Key;
            dataTransferLayer.LayerStandart = item.Value;
            MainForm.listDataLayers.Add(dataTransferLayer);

         }
         serialize.SerializeList(MainForm.listDataLayers.GroupBy(x => x.LayerDocument).Select(q => q.First()).ToList());
      }

      public void GetListNotStandartLayers(Dictionary<string, string> dicForTransferLayer, List<TranferLayer> listDataLayers, ListView listViewTransfer, ListBox listBoxLayersDoc)
      {
         foreach (string layer in MainForm.activeList)
         {
            if ((layer.Contains("|")) || (layer == "0") || (layer == "Defpoints"))                              //игнор данных слоев
            {
               MainForm.activeList = MainForm.activeList.Select(x => x).Where(x => x != (layer)).ToList();
               continue;
            }
            else if (listDataLayers.Exists(x => x.LayerDocument.Equals(layer)))
            {
               string layerData = listDataLayers.Select(x => x).Where(x => x.LayerDocument.Equals(layer)).ToList()[0].LayerStandart;            //Проверка на наличие слоя в базе слоев
               listViewTransfer.Items.Add(new ListViewItem(new string[] { layer, layerData }));
               MainForm.activeList = MainForm.activeList.Select(x => x).Where(x => x != (layer)).ToList();
            }
            else if (MainForm.standartList.Exists(x => x.Equals(layer)))                                                        //получение слоев, совпадающих по названию со стандартными
            {
               MainForm.activeList = MainForm.activeList.Select(x => x).Where(x => x != (layer)).ToList();
               dicForTransferLayer.Add(layer, layer);
            }

            else
            {
               listBoxLayersDoc.Items.Add(layer);                                                                 //заполнение ListBox со слоями активного документа
            }
         }
      }

      public List<string> GetListLayersActiveDocument()
      {
         List<string> activeList = new List<string>();
         activeList = (GetListLayers(Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, false))
             .Select(x => x.Name).ToList();                                                                                             //Список слоев активного документа
         activeList.Sort();
         return activeList;
      }

      public List<string> GetlistLayersStandart(Database stdDatabase, ListBox listBoxLayersStd)
      {
         //  PathToStadartFile += "\\" + GetMyCodeGroup() + "\\" + GetMyCodeGroup() + ".dws";
         List<string> standartList = new List<string>();
         stdDatabase.ReadDwgFile(PathToStadartFile, FileShare.Read, true, "");   //База стандарта
         standartList = (GetListLayers(stdDatabase, false)).Select(x => x.Name).ToList();                             //Список слоев стандарта
         standartList.Sort();
         foreach (string layer in standartList)
         {
            listBoxLayersStd.Items.Add(layer);                                                                  //заполнение ListBox со слоями стандарта
         }
         return standartList;
      }

      //public static string GetMyCodeGroup()
      //{
      //   string myCodeGroup = "O_o";
      //   AcadPreferences preferences = null;
      //   try
      //   {
      //      preferences = (AcadPreferences)AutoCadApp.Preferences;
      //      string[] myCodeGroupMas = preferences.Files.TemplateDwgPath.Split('\\');
      //      if (myCodeGroupMas[myCodeGroupMas.Length - 1] == "")
      //      {
      //         myCodeGroup = myCodeGroupMas[myCodeGroupMas.Length - 2];
      //      }
      //      else
      //      {
      //         myCodeGroup = myCodeGroupMas[myCodeGroupMas.Length - 1];
      //      }

      //   }
      //   catch { }

      //   return myCodeGroup;
      //}
   }
}
