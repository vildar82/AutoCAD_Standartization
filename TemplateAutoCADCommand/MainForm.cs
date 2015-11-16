using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AutoCAD_Standartization
{
   public partial class MainForm : Form
   {
      public static List<TranferLayer> listDataLayers;
      LayerManager lm = new LayerManager();
      public static List<string> standartList;
      public static List<string> activeList;
      public static MainForm mf;
      public static Dictionary<long, string> dicLineTypes = new Dictionary<long, string>();

      public MainForm()
      {
         InitializeComponent();
         //lm = new LayerManager();
      }

      private void MainForm_Load(object sender, EventArgs e)
      {
         dicLineTypes = new Dictionary<long, string>();
         Dictionary<string, string> dicForTransferLayer = new Dictionary<string, string>();
         using (var stdDatabase = new Database(false, true))
         {
            standartList = lm.GetlistLayersStandart(stdDatabase, listBoxLayersStd);
            activeList = lm.GetListLayersActiveDocument();
            SerializerXml ser = new SerializerXml();

            listDataLayers = ser.DeserializeXmlFile();                                             //Список слоев базы
            lm.GetListNotStandartLayers(dicForTransferLayer, listDataLayers, listViewTransfer, listBoxLayersDoc);
            groupBoxActive.Text = "Нестандартные слои в чертеже (" + activeList.Count + ")";
            groupBoxStd.Text = "Стандартные слои (" + standartList.Count + ")";
            if (dicForTransferLayer.Count == 0) return;
            lm.TransferLayer(dicForTransferLayer);                                                                      //Замена свойств слоя, совпадающего по названию со стандартным
         }
      }

      private void listBoxLayersDoc_SelectedIndexChanged(object sender, EventArgs e)
      {
         string selectedLayer = listBoxLayersDoc.GetItemText(listBoxLayersDoc.SelectedItem);
         ObjectIdCollection objects = lm.GetEntitiesOnLayer(selectedLayer);                                                //Кол-во объектов, входящих в слой
         labelCountElements.Text = objects.Count.ToString();
      }

      private void buttonSelectForTransfer_Click(object sender, EventArgs e)
      {
         if ((listBoxLayersDoc.SelectedItem == null) || (listBoxLayersStd.SelectedItem == null)) return;

         string activeLayer = listBoxLayersDoc.GetItemText(listBoxLayersDoc.SelectedItem);
         string stdLayer = listBoxLayersStd.GetItemText(listBoxLayersStd.SelectedItem);
         ListViewItem item = new ListViewItem(new string[] { activeLayer, stdLayer });
         listViewTransfer.Items.Add(item);
         listBoxLayersDoc.Items.RemoveAt(listBoxLayersDoc.SelectedIndex);
         activeList = activeList.Select(X => X).Where(X => X != activeLayer).ToList();
         groupBoxActive.Text = "Нестандартные слои в чертеже (" + listBoxLayersDoc.Items.Count + ")";
      }

      private void buttonTransfer_Click(object sender, EventArgs e)
      {
         bool isEnter = false;
         Dictionary<string, string> dicLayers = new Dictionary<string, string>();
         for (int i = listViewTransfer.Items.Count - 1; i >= 0; i--)
         {
            dicLayers.Add(listViewTransfer.Items[i].Text, listViewTransfer.Items[i].SubItems[1].Text);               //Список слоев на изменение по стандарту
            listViewTransfer.Items.RemoveAt(i);
         }
         if (dicLayers.Count != 0)
         {
            isEnter = true;
            lm.TransferLayer(dicLayers);                                                                                      //Трансфер выбранных слоев
            lm.SaveDataLayers(dicLayers);
         }
         if (checkBoxTextStyle.Checked)
         {
            isEnter = true;
            TextStyleManager txtManager = new TextStyleManager();
            txtManager.TransferTextStyles();
         }
         if (checkBoxSizeStyle.Checked)
         {
            isEnter = true;
            DimStyleManager dimManager = new DimStyleManager();
            dimManager.TransferDimStyles();
         }
         if (!isEnter) return;

         //object aDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
         //aDoc.GetType().InvokeMember("Audit",
         //    BindingFlags.InvokeMethod, null,
         //    aDoc, new object[] { true });

         MessageBox.Show("Стандартизация чертежа завершена!", "Выполнено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      private void buttonDelete_Click(object sender, EventArgs e)
      {
         if (listViewTransfer.SelectedItems.Count == 0) return;
         string deletedLayerName = listViewTransfer.Items[listViewTransfer.SelectedItems[0].Index].Text;
         activeList.Add(deletedLayerName);

         groupBoxActive.Text = "Нестандартные слои в чертеже (" + activeList.Count + ")";
         listDataLayers = listDataLayers.Select(x => x).Where(x => x.LayerDocument != deletedLayerName).ToList();        //Удаление слоя из списка настроек
         listViewTransfer.Items.RemoveAt(listViewTransfer.SelectedItems[0].Index);                                      //Удаление слоя из списка на трансфер
         listBoxLayersDoc.Items.Clear();
         foreach (string item in activeList)
         {
            listBoxLayersDoc.Items.Add(item);
         }
      }
   }
}
