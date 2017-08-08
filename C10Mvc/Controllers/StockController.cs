using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using C10Mvc.Class;
using M10.lib.model;

namespace C10Mvc.Controllers
{
  [RoutePrefix("StockApi")]
  public class StockController : BaseApiController
  {

    [HttpGet]
    [Route("getStockType")]
    public StockInfo getStockType()
    {
      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;
      
      NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);


      string ssql = " select * from stockinfo where 1=1 ";
      if (Params["StockCode"] != null)
      {
        ssql += string.Format(" and stockcode = '{0}' ", Params["StockCode"]);
      }


      StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(ssql);

      return StockInfoItem;
    }




  }
}
