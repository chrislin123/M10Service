using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M10Api.Class;
using System.Web;
using M10.lib;

namespace M10Api.Controllers
{
  [RoutePrefix("HighRailApi")]
  public class HighRailController : BaseApiController
  {


    [HttpGet]
    [Route("getHighRailData")]
    public List<dynamic> getStationData()
    {

      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      NameValueCollection Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);
      

      string ssql = " select * from highrail where 1=1 ";
      if (Params["type"] != null)
      {
        ssql += string.Format(" and type = '{0}' ", Params["type"]);
      }
      if (Params["country"] != null)
      {
        ssql += string.Format(" and country = '{0}' ", Params["country"]);
      }
      if (Params["week"] != null)
      {
        ssql += string.Format(" and week = '{0}' ", Params["week"]);
      }


      var list = dbDapper.Query(ssql);

      return list;
    }
  }
}
