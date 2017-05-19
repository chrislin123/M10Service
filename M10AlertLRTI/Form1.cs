using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CL.Data;
using System.Configuration;
using M10.lib;

namespace M10AlertLRTI
{
  public partial class Form1 : BaseForm
  {
    //public string ssql = string.Empty;
    //public string sConnectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.vghtc].ConnectionString;
    //public ODAL oDal = new ODAL(Properties.Settings.Default.vghtc);
    //public DALDapper dbDapper;


    public Form1()
    {
      InitializeComponent();

      base.InitForm();
      //ConnectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.vghtc].ConnectionString;
      //oDal = new ODAL(Properties.Settings.Default.vghtc);
      //DbDapper = new DALDapper(ConnectionString);



    }
  }
}
