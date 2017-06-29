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
using M10.lib;
using System.Reflection;
using System.ComponentModel;
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

    public ActionResult checkPass(string pass)
    {
      dynamic result = new ExpandoObject();
      result.pass = "N";

      ssql = " select * from LRTIAlertMail where type = 'usps' and value = @value ";
      int iCount = dbDapper.QueryTotalCount(ssql, new { value = pass.ToUpper() });

      if (iCount>0)
      {
        result.pass = "Y";
      }

      return Content(JsonConvert.SerializeObject(result), "application/json");
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



    public ActionResult DownRainCountry(string sd,string ed,string country)
    {
      DateTime dtStart;
      DateTime dtEnd;

      if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
      {
        dtStart = DateTime.Now;
        dtEnd = DateTime.Now;
      }
      else
      {
        string[] aSdt = sd.Replace(' ', '-').Split('-');
        string[] aEdt = ed.Replace(' ', '-').Split('-');

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
      }

      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

      sStartDate = "2016-04-08T00:00:01";
      sEndDate = "2017-04-09T00:00:01";
      //country = "桃園市";

      string ssql = @" select * from RainStation where 1=1
                      and RTime between @sd and @ed
                      and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                        ";
      if (country != "全部") ssql += " and COUNTY = @COUNTY ";
      ssql += " order by RTime ";

      List<RainStation> DataList = new List<RainStation>();
      DataList = dbDapper.Query<RainStation>(ssql, new { COUNTY = country, sd = sStartDate, ed = sEndDate });
     
      //產生檔案路徑
      string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
      //建立資料夾
      Directory.CreateDirectory(sTempPath);
      string sSaveFilePath = Path.Combine(sTempPath, "RainDataByCountry_" + Guid.NewGuid().ToString() + ".xlsx");


      List<string> PropertyNameList = GetPropertyName<RainStation>().ToList<string>();

      



      DataTable dt = new DataTable();


      dt = ConvertToDataTable<RainStation>(DataList);


      foreach (string item in PropertyNameList)
      {
        dt.Columns.Add(item);
      }

      //dt.Columns.Add("STID");
      //dt.Columns.Add("STNAME");
      //dt.Columns.Add("LAT");
      //dt.Columns.Add("LON");
      //dt.Columns.Add("WGS84_lon");
      //dt.Columns.Add("WGS84_lat");
      //dt.Columns.Add("ELEV");
      //dt.Columns.Add("RTime");
      //dt.Columns.Add("MIN10");
      //dt.Columns.Add("RAIN");
      //dt.Columns.Add("Hour2");
      //dt.Columns.Add("HOUR3");
      //dt.Columns.Add("HOUR6");
      //dt.Columns.Add("HOUR12");
      //dt.Columns.Add("HOUR24");
      //dt.Columns.Add("NOW");
      //dt.Columns.Add("Day1");
      //dt.Columns.Add("Day2");
      //dt.Columns.Add("Day3");
      //dt.Columns.Add("COUNTY");
      //dt.Columns.Add("TOWN");
      //dt.Columns.Add("ATTRIBUTE");
      //dt.Columns.Add("STATUS");
      //dt.Columns.Add("DebrisRefStation");
      //dt.Columns.Add("RT");
      //dt.Columns.Add("LRTI");
      //dt.Columns.Add("WLRTI");

      foreach (RainStation item in DataList)
      {
        DataRow NewRow = dt.NewRow();
        
        NewRow["STID"] = item.STID;
        NewRow["STNAME"] = item.STNAME;
        NewRow["LAT"] = item.LAT;
        NewRow["LON"] = item.LON;
        NewRow["WGS84_lon"] = item.WGS84_lon;
        NewRow["WGS84_lat"] = item.WGS84_lat;
        NewRow["ELEV"] = item.ELEV;
        NewRow["RTime"] = item.RTime;
        NewRow["MIN10"] = item.MIN10;
        NewRow["RAIN"] = item.RAIN;
        NewRow["Hour2"] = item.Hour2;
        NewRow["HOUR3"] = item.HOUR3;
        NewRow["HOUR6"] = item.HOUR6;
        NewRow["HOUR12"] = item.HOUR12;
        NewRow["HOUR24"] = item.HOUR24;
        NewRow["NOW"] = item.NOW;
        NewRow["Day1"] = item.Day1;
        NewRow["Day2"] = item.Day2;
        NewRow["Day3"] = item.Day3;
        NewRow["COUNTY"] = item.COUNTY;
        NewRow["TOWN"] = item.TOWN;
        NewRow["ATTRIBUTE"] = item.ATTRIBUTE;
        NewRow["STATUS"] = item.STATUS;
        NewRow["DebrisRefStation"] = item.DebrisRefStation;
        NewRow["RT"] = item.RT;
        NewRow["LRTI"] = item.LRTI;
        NewRow["WLRTI"] = item.WLRTI;     

        dt.Rows.Add(NewRow);
      }

      DataExport de = new DataExport();
      de.ExportBigDataToExcel(sSaveFilePath, dt);

      if (System.IO.File.Exists(sSaveFilePath))
      {
        string filename = string.Format("RainDataByCountry_{0}_{1}.xlsx", sd, ed);
        filename = filename.Replace("-", "");
        //讀成串流
        Stream iStream = new FileStream(sSaveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        //回傳出檔案
        return File(iStream, "application/vnd.ms-excel", filename);
      }

      return View();
    }

    public DataTable ConvertToDataTable<T>(IList<T> data)
    {
      PropertyDescriptorCollection properties =
         TypeDescriptor.GetProperties(typeof(T));
      DataTable table = new DataTable();
      foreach (PropertyDescriptor prop in properties)
        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
      foreach (T item in data)
      {
        DataRow row = table.NewRow();
        foreach (PropertyDescriptor prop in properties)
          row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        table.Rows.Add(row);
      }
      return table;
    }


    public static IEnumerable<string> GetPropertyName<T>()
    {
      var prof = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      return prof.Select(p => p.Name);
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