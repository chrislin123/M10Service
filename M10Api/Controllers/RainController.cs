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
      ViewBag.forecastdate = "";
      ssql = " select MAX(RTime) as RTime from RunTimeRainData ";
      object oforecastdate = dbDapper.ExecuteScale(ssql);
      if (oforecastdate!= null)
      {
        ViewBag.forecastdate = dbDapper.ExecuteScale(ssql).ToString();
      }
      

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

    public JsonResult GetStationDDL(string STID)
    {
      List<SelectListItem> items = new List<SelectListItem>();


      ssql = @" select * from StationData ";
      if (string.IsNullOrEmpty(STID) == false) ssql += " where STID like @STID ";
      ssql += " order by STID ";

      var Stations = dbDapper.Query(ssql, new { STID = "%" + STID + "%" });

      foreach (var item in Stations)
      {
        items.Add(new SelectListItem()
        {
          Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
          Value = item.STID
        });
      }



      //if (Stations.Count != 0)
      //{
      //  items.Insert(0, new SelectListItem() { Text = "全部", Value = "全部" });
      //}

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

    public ActionResult getAlertSet()
    {
      dynamic result = new ExpandoObject();


      ssql = " select * from LRTIAlertMail where type = 'isal' ";
      LRTIAlertMail inst = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);

      result.AlertSet = inst.value;

      return Content(JsonConvert.SerializeObject(result), "application/json");
    }

    public ActionResult checkPass(string pass)
    {
      dynamic result = new ExpandoObject();
      result.pass = "N";

      ssql = " select * from LRTIAlertMail where type = 'usps' and value = @value ";
      int iCount = dbDapper.QueryTotalCount(ssql, new { value = pass.ToUpper() });

      if (iCount > 0)
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



    public ActionResult DownRainCountry(string sd, string ed, string country)
    {
      DateTime dtStart;
      DateTime dtEnd;

      sd = sd.Replace(' ', '-');
      ed = ed.Replace(' ', '-');

      if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
      {
        dtStart = DateTime.Now;
        dtEnd = DateTime.Now;
      }
      else
      {
        string[] aSdt = sd.Split('-');
        string[] aEdt = ed.Split('-');

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
      }

      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

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
      string sSaveFilePath = Path.Combine(sTempPath, "RainDataByCountry_" + Guid.NewGuid().ToString() + ".csv");

      DataTable dt = Utils.ConvertToDataTable<RainStation>(DataList);


      DataExport de = new DataExport();
      Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

      if (bSuccess)
      {
        string filename = string.Format("RainDataByCountry_{0}_{1}.csv", sd, ed).Replace("-", "");

        //ASP.NET 回應大型檔案的注意事項
        //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


        //***** 下載檔案過大，使用特殊方法 *****
        HttpContext context = System.Web.HttpContext.Current;
        context.Response.TransmitFile(sSaveFilePath);
        context.Response.ContentType = "text/csv";
        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
        Response.End();
      }

      return null;
    }

    public ActionResult DownRainStation(string sd, string ed, string Station)
    {
      DateTime dtStart;
      DateTime dtEnd;

      sd = sd.Replace(' ', '-');
      ed = ed.Replace(' ', '-');

      if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
      {
        dtStart = DateTime.Now;
        dtEnd = DateTime.Now;
      }
      else
      {
        string[] aSdt = sd.Split('-');
        string[] aEdt = ed.Split('-');

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
      }

      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

      string ssql = @" select * from RainStation where 1=1
                      and STID = @STID
                      and RTime between @sd and @ed
                      and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                       order by RTime   ";

      List<RainStation> DataList = new List<RainStation>();
      DataList = dbDapper.Query<RainStation>(ssql, new { STID = Station, sd = sStartDate, ed = sEndDate });

      //產生檔案路徑
      string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
      //建立資料夾
      Directory.CreateDirectory(sTempPath);
      string sSaveFilePath = Path.Combine(sTempPath, "RainDataByStation_" + Guid.NewGuid().ToString() + ".csv");

      DataTable dt = Utils.ConvertToDataTable<RainStation>(DataList);


      DataExport de = new DataExport();
      Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

      if (bSuccess)
      {
        string filename = string.Format("RainDataByStation_{0}_{1}.csv", sd, ed).Replace("-", "");

        //ASP.NET 回應大型檔案的注意事項
        //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


        //***** 下載檔案過大，使用特殊方法 *****
        HttpContext context = System.Web.HttpContext.Current;
        context.Response.TransmitFile(sSaveFilePath);
        context.Response.ContentType = "text/csv";
        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
        Response.End();
      }

      return null;
    }

    public ActionResult DownSumRain(string sd, string ed)
    {
      DateTime dtStart;
      DateTime dtEnd;

      sd = sd.Replace(' ', '-');
      ed = ed.Replace(' ', '-');

      if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
      {
        dtStart = DateTime.Now;
        dtEnd = DateTime.Now;
      }
      else
      {
        string[] aSdt = sd.Split('-');
        string[] aEdt = ed.Split('-');

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
      }

      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

      string ssql = @" select STID,STNAME,COUNTY,LAT,LON,sum(CONVERT(float, RAIN)) as RAIN from RainStation where 1=1
                       and RTime between @sd and @ed
                       and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                       group by STID,STNAME,COUNTY,LAT,LON   ";

      //List<RainStation> DataList = new List<RainStation>();
      var DataList = dbDapper.Query(ssql, new { sd = sStartDate, ed = sEndDate });

      //產生檔案路徑
      string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
      //建立資料夾
      Directory.CreateDirectory(sTempPath);
      string sSaveFilePath = Path.Combine(sTempPath, "SumRainData_" + Guid.NewGuid().ToString() + ".csv");

      DataTable dt = new DataTable();
      dt.Columns.Add("STID");
      dt.Columns.Add("STNAME");
      dt.Columns.Add("COUNTY");
      dt.Columns.Add("LAT");
      dt.Columns.Add("LON");
      dt.Columns.Add("RAIN");

      foreach (var item in DataList)
      {
        DataRow NewRow = dt.NewRow();
        NewRow["STID"] = item.STID;
        NewRow["STNAME"] = item.STNAME;
        NewRow["COUNTY"] = item.COUNTY;
        NewRow["LAT"] = item.LAT;
        NewRow["LON"] = item.LON;
        NewRow["RAIN"] = item.RAIN;

        dt.Rows.Add(NewRow);
      }

      DataExport de = new DataExport();
      Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

      if (bSuccess)
      {
        string filename = string.Format("SumRainData_{0}_{1}.csv", sd, ed).Replace("-", "");

        //ASP.NET 回應大型檔案的注意事項
        //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


        //***** 下載檔案過大，使用特殊方法 *****
        HttpContext context = System.Web.HttpContext.Current;
        context.Response.TransmitFile(sSaveFilePath);
        context.Response.ContentType = "text/csv";
        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
        Response.End();
      }

      return null;
    }

    public ActionResult ExpRainStation()
    {
      return View();
    }

    public ActionResult ExpStationCoord()
    {
      return View();
    }

    public bool xDownload(string xFile, string out_file)
    {
      if (System.IO.File.Exists(xFile))
      {
        try
        {
          FileInfo xpath_file = new FileInfo(xFile);
          System.Web.HttpContext.Current.Response.Clear(); //清除buffer
          System.Web.HttpContext.Current.Response.ClearHeaders(); //清除 buffer 表頭
          System.Web.HttpContext.Current.Response.Buffer = false;
          System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
          // 檔案類型還有下列幾種"application/pdf"、"application/vnd.ms-excel"、"text/xml"、"text/HTML"、"image/JPEG"、"image/GIF"
          //System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(out_file, System.Text.Encoding.UTF8));
          System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(xpath_file.Name, System.Text.Encoding.UTF8));
          // 考慮 utf-8 檔名問題，以 out_file 設定另存的檔名
          System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", xpath_file.Length.ToString()); //表頭加入檔案大小
          System.Web.HttpContext.Current.Response.WriteFile(xpath_file.FullName);

          // 將檔案輸出
          System.Web.HttpContext.Current.Response.Flush();
          // 強制 Flush buffer 內容
          System.Web.HttpContext.Current.Response.End();
          return true;

        }
        catch (Exception)
        { return false; }

      }
      else
        return false;
    }

  }
}