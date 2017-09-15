using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using M10.lib;

namespace C10Mvc.Class
{
  public class BaseApiController : ApiController
  {
    public string ssql = string.Empty;
    private DALDapper _dbDapper;
    private string _ConnectionString;
    private StockUtil _StockUtil;
    private StockHelper _stockhelper;
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

    public StockUtil oStockUtil
    {
      get
      {
        if (_StockUtil == null)
        {
          _StockUtil = new StockUtil();
        }

        return _StockUtil;
      }
    }

    public StockHelper stockhelper
    {
      get
      {
        if (_stockhelper == null)
        {
          _stockhelper = new StockHelper();
        }
        return _stockhelper;
      }

    }

  }
}