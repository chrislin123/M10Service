using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M10.lib.model;

namespace M10Tools
{
  public partial class MainForm : BaseForm
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void btnImpErrLRTI_Click(object sender, EventArgs e)
    {

      DataTable dtresult = new DataTable();
      dtresult.Columns.Add("STID");
      dtresult.Columns.Add("ELRTI");

      string sFilePath = @"C:\ErrLRTI.csv";

      List<string> ll = new List<string>();


      Dictionary<string, string> dd = new Dictionary<string, string>();

      using (StreamReader SR = new StreamReader(sFilePath, System.Text.Encoding.Default))
      {
        string Line;
        while ((Line = SR.ReadLine()) != null)
        {

          string[] sSplit = Line.Split('|');

          if (dd.ContainsKey(sSplit[3]) == false)
          {
            dd.Add(sSplit[3], sSplit[5]);
          }

          ll.Add(Line);
        }
      }

      foreach (KeyValuePair<string, string> item in dd)
      {
        StationErrLRTI Temp = new StationErrLRTI();
        Temp.STID = item.Key;
        Temp.ELRTI = item.Value;

        dbDapper.Insert(Temp);
      }

    }
  }
}
