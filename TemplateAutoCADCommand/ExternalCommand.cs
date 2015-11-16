using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: CommandClass(typeof(AutoCAD_Standartization.ExternalCommand))]

namespace AutoCAD_Standartization
{
   public class ExternalCommand
   {
      [CommandMethod("Standartization", CommandFlags.Modal)]
      public void Standartization()
      {
         MainForm mf = new MainForm();
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(mf);         
      }      
   }
}
