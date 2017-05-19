using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CL.Data;
using System.Configuration;
using M10.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using CL.Data;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing; //word
using DocumentFormat.OpenXml.Spreadsheet; //excel
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Configuration;
using M10.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M10AlertLRTI
{
  public class BaseForm : Form
  {
    public string ssql = string.Empty;
    private string sConnectionString;
    private ODAL _Dal;
    private DALDapper dbDapper;

    public DALDapper DbDapper
    {
      get
      {
        if (dbDapper == null)
        {
          dbDapper = new DALDapper(ConnectionString);
        }

        return dbDapper;
      }
    }

    public string ConnectionString
    {
      get
      {
        if (sConnectionString == "")
        {
          sConnectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.vghtc].ConnectionString;
        }
        
        return sConnectionString;
      }
    }

    public ODAL oDal
    {
      get
      {
        if (_Dal == null)
        {
          _Dal = new ODAL(Properties.Settings.Default.vghtc);
        }
        return _Dal;
      }
    }
  }
}
