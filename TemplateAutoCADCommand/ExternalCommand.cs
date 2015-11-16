using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoCAD_PIK_Manager;
using System.Reflection;

[assembly: CommandClass(typeof(AutoCAD_Standartization.ExternalCommand))]

namespace AutoCAD_Standartization
{
   public class ExternalCommand
   {
      [CommandMethod("Standartization", CommandFlags.Modal)]
      public void Standartization()
      {
         try
         {
            Log.Info("Start Command Standartization {0}", Assembly.GetExecutingAssembly().GetName().Version);
            MainForm mf = new MainForm();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(mf);
         }
         catch (System.Exception ex)
         {
            Log.Error(ex, "Standartization");
         }
      }      
   }
}
