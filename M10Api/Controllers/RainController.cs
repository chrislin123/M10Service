using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M10Api.Class;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using M10.lib;
using System.Data;
//using System.Web.Http;

namespace M10Api.Controllers
{
  public class RainController : BaseController
  {
    // GET: Rain
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult QueryRain(string cityId)
    {


      string cityid = Request.QueryString["cityId"];
      //if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      //var Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);

      //string sType = Params["type"];


      if (Request.Form["SelectItems"] != null)
      {
        string sss = Request.Form["SelectItems"].ToString();
      }
      

      //string ssql = @" select * from LRTIAlert where status = '{0}' order by country,town ";
      ssql = @"   select CONVERT(float, MIN10) as MIN10
                        ,CONVERT(float, RAIN) as RAIN
                        ,CONVERT(float, HOUR3) as HOUR3
                        ,CONVERT(float, HOUR6) as HOUR6
                        ,CONVERT(float, HOUR12) as HOUR12
                        ,CONVERT(float, HOUR24) as HOUR24
                        ,CONVERT(float, NOW) as NOW
                        ,ROUND(CONVERT(float, RT),2) as RT
                        ,CONVERT(float, LRTI) as LRTI
                        ,ROUND(CONVERT(float, b.ELRTI),2) as ELRTI
                        ,* from RunTimeRainData a
                        left join StationErrLRTI b on a.STID = b.STID 
                    ";
      List<dynamic> data = new List<dynamic>();

      data = dbDapper.Query(ssql);


      foreach (var LoopItem in data)
      {
        if (LoopItem.STATUS == "-99")
        {
          LoopItem.MIN10 = "0";
          LoopItem.RAIN = "0";
          LoopItem.HOUR3 = "0";
          LoopItem.HOUR6 = "0";
          LoopItem.HOUR12 = "0";
          LoopItem.HOUR24 = "0";
          LoopItem.NOW = "0";
          LoopItem.RT = "0";
          LoopItem.LRTI = "0";

          LoopItem.STATUS = "異常";
        }

        double dLRTI = 0;
        double dELRTI = 0;
        double.TryParse(Convert.ToString(LoopItem.LRTI), out dLRTI);
        double.TryParse(Convert.ToString(LoopItem.ELRTI), out dELRTI);

        LoopItem.LStatus = "";
        if (dLRTI > dELRTI)
        {
          LoopItem.LStatus = "Red";
        }
      }

      //取得更新最新時間
      ssql = " select MAX(RTime) as RTime from RunTimeRainData ";
      ViewBag.forecastdate = dbDapper.ExecuteScale(ssql).ToString();

      ViewBag.count = data.Count;
      ViewData["RunTimeRainData"] = data;




      var vSelectItems = dbDapper.Query(" select distinct COUNTY from StationData order by COUNTY ");


      List<SelectListItem> sli = new List<SelectListItem>();

      foreach (var item in vSelectItems)
      {
        sli.Add(new SelectListItem() { Text = item.COUNTY, Value = item.COUNTY });
      }
      

      //SelectList sl = new SelectList(vSelectItems, "COUNTY", "COUNTY");
      
      ViewBag.SelectItems = sli;



      return View();
    }

    public JsonResult GetCountyDDL(string cityId)
    {
      List<SelectListItem> items = new List<SelectListItem>();

      var Countrys = dbDapper.Query(" select distinct COUNTY from StationData order by COUNTY ");

      foreach (var item in Countrys)
      {
        items.Add(new SelectListItem()
        {
          Text = item.COUNTY,
          Value = item.COUNTY
        });
      }

      if (Countrys.Count != 0)
      {
        items.Insert(0, new SelectListItem() { Text = "全部", Value = "全部" });
      }

      return this.Json(items, JsonRequestBehavior.AllowGet);
    }


    public ActionResult ExportRain()
    {
      return View();
    }

    public ActionResult QueryRTI()
    {
      return View();
    }

    public ActionResult QueryRTI3()
    {
      return View();
    }


  }
}