﻿
using System.Windows.Forms;

using System.Configuration;
using M10.lib;


namespace M10Tools
{
  public class BaseForm : Form
  {
    public string ssql = string.Empty;
    private string _ConnectionString;
    //public ODAL oDal;
    private DALDapper _dbDapper;

    public DALDapper dbDapper
    {
      get
      {
        if (_dbDapper == null)
        {
          _dbDapper = new DALDapper(ConnectionString);
        }

        return _dbDapper;
      }
    }

    public string ConnectionString
    {
      get
      { 
        if (string.IsNullOrEmpty(_ConnectionString))
        {
          _ConnectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]].ConnectionString;          
        }
        
        return _ConnectionString;
      }
    }

    //1060519 開放後，介面切換會當機(問題出在cl.data.odal.cs的解構子~ODAL(){}中，尚未無解)
    //public ODAL oDal
    //{
    //  get
    //  {
    //    if (_Dal == null)
    //    {
    //      _Dal = new ODAL(Properties.Settings.Default.vghtc);
    //    }
    //    return _Dal;
    //  }
    //}


    public BaseForm()
    {

    }

    
    public void InitForm()
    {
      _ConnectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.DBDefault].ConnectionString;
      _dbDapper = new DALDapper(_ConnectionString);
      //oDal = new ODAL(Properties.Settings.Default.DBDefault);
      //logger = NLog.LogManager.GetCurrentClassLogger();
    }

  }
}
