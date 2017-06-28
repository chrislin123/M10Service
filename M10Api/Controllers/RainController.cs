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
using M10.lib.model;
using System.Data;
using System.Dynamic;
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
      //string cityid1 = Request.QueryString["cityId"];
      ViewBag.SelectCountry = "全部";

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

      if (string.IsNullOrEmpty(cityId) == false)
      {
        ViewBag.SelectCountry = cityId;

        if (cityId != "全部")
        {
          ssql += "where COUNTY = @cityId ";
        }
      }

      List<dynamic> data = dbDapper.Query(ssql, new { cityId = cityId });
      foreach (var LoopItem in data)
      {
        LoopItem.LStatus = "";

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
          //設定顏色
          LoopItem.LStatus = "LightGray";
        }

        double dLRTI = 0;
        double dELRTI = 0;
        double.TryParse(Convert.ToString(LoopItem.LRTI), out dLRTI);
        double.TryParse(Convert.ToString(LoopItem.ELRTI), out dELRTI);

        
        if (dLRTI > dELRTI && LoopItem.ELRTI != null)
        {
          LoopItem.LStatus = "Red";
        }
      }

      //取得更新最新時間
      ssql = " select MAX(RTime) as RTime from RunTimeRainData ";
      ViewBag.forecastdate = dbDapper.ExecuteScale(ssql).ToString();

      //資料筆數
      ViewBag.count = data.Count;
      ViewData["RunTimeRainData"] = data;

     

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


    public ActionResult postAlertSet()
    {
      dynamic result = new ExpandoObject();

      ssql = " select * from LRTIAlertMail where type = 'isal' ";
      LRTIAlertMail inst = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);

      if (inst.value == "Y")
      {
        inst.value = "N";
      }
      else
      {
        inst.value = "Y";
      }

      //更新成功才回傳
      if (dbDapper.Update(inst))
      {
        result.result = "OK";
        result.AlertSet = inst.value;
      }
      
      return Content(JsonConvert.SerializeObject(result), "application/json");
      //return Json(JsonConvert.SerializeObject(result), "application/json", JsonRequestBehavior.AllowGet);

      //return this.Json(result, JsonRequestBehavior.AllowGet);
      //return JsonConvert.SerializeObject(result);
    }

    public JsonResult getAlertSet()
    {
      dynamic result = new ExpandoObject();


      ssql = " select * from LRTIAlertMail where type = 'isal' ";
      LRTIAlertMail inst = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);
      
      result.AlertSet = inst.value;
      
      return this.Json(result, JsonRequestBehavior.AllowGet);
    }


    public ActionResult ExportRain()
    {
      return View();
    }

    public ActionResult QueryRTI(string type)
    { 
      ViewBag.type = "0";
      if (string.IsNullOrEmpty(type) == false) ViewBag.type = type;

      ssql = " select * from RtiDetail where delaytime = @type  order by station ";

      List<dynamic> data = dbDapper.Query(ssql, new { type = ViewBag.type });

      //資料筆數
      ViewBag.count = data.Count;
      ViewData["RtiDetail"] = data;

      //
      return View();
    }

    public ActionResult QueryRTI3(string type)
    {
      ViewBag.type = "0";
      if (string.IsNullOrEmpty(type) == false) ViewBag.type = type;

      ssql = " select * from Rti3Detail where delaytime = @type order by station ";
      List<dynamic> data = dbDapper.Query(ssql, new { type = ViewBag.type });

      //資料筆數
      ViewBag.count = data.Count;
      ViewData["Rti3Detail"] = data;

      return View();
    }

    public ActionResult ExpRainCountry()
    {
      return View();
    }

    public ActionResult DownRainCountry()
    {
      return View();
    }

    public ActionResult ExpRainStation()
    {
      return View();
    }

    public ActionResult ExpStationCoord()
    {
      return View();
    }

  }
}