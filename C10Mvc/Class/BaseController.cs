using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Configuration;
using M10.lib;

namespace C10Mvc.Class
{
  public class BaseController : Controller
  {
    public string ssql = string.Empty;
    private DALDapper _dbDapper;
    private string _ConnectionString;


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

  }
}