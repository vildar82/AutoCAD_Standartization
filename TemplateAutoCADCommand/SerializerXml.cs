using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace AutoCAD_Standartization
{
    class SerializerXml
    {
        LayerManager lm = new LayerManager();
        public void SerializeList(List<TranferLayer> listTransfer)
        {
            using (FileStream fs = new FileStream(lm.PathToXMLFile, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer ser = new XmlSerializer(listTransfer.GetType());
             
                ser.Serialize(fs, listTransfer);
            }
        }

        public List<TranferLayer> DeserializeXmlFile()
        {
            List<TranferLayer> dataLayers = new List<TranferLayer>();
            try
            {
                XmlSerializer ser = new XmlSerializer(dataLayers.GetType());
                if (!File.Exists(lm.PathToXMLFile))
                    return dataLayers;
                using (XmlReader reader = XmlReader.Create(lm.PathToXMLFile))
                {
                    try
                    {
                        dataLayers = (List<TranferLayer>)ser.Deserialize(reader);
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          

            return dataLayers;
        }
    }
}
