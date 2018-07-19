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
  public class WarnController : BaseController
  {
    // GET: Warn
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult warnlist()
    {

      try
      {
        var AlertUpdateTm = dbDapper.ExecuteScale(@" select value from LRTIAlertMail where type = 'altm' ");
        ViewBag.forecastdate = AlertUpdateTm == null ? "" : AlertUpdateTm.ToString();

        string ssql = @" select * from LRTIAlert where status = '{0}' order by country,town ";
        //新增
        var dataI = dbDapper.Query(string.Format(ssql, "I"));
        //持續
        var dataC = dbDapper.Query(string.Format(ssql, "C"));
        //解除
        var dataD = dbDapper.Query(string.Format(ssql, "D"));


        List<dynamic> data = new List<dynamic>();
        data.AddRange(dataI);
        data.AddRange(dataC);
        data.AddRange(dataD);

        foreach (var item in data)
        {
          //處理狀態改中文顯示
          if (item.status == "I") item.status = M10Const.AlertStatus.I;
          if (item.status == "C") item.status = M10Const.AlertStatus.C;
          if (item.status == "O") item.status = M10Const.AlertStatus.O;
          if (item.status == "D") item.status = M10Const.AlertStatus.D;

          //處理ELRTI取至小數第二位
          decimal dELRTI = 0;
          if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
          {
            item.ELRTI = Math.Round(dELRTI, 2).ToString();
          }

          decimal dRT = 0;
          if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
          {
            item.RT = Math.Round(dRT, 2).ToString();
          }

        }


        ViewBag.count = data.Count;
        ViewData["LRTIAlert"] = data;
      }
      catch (Exception ex)
      {
        Logger.Log(NLog.LogLevel.Error, ex.Message);
      }

      return View();
    }

    
    public ActionResult warnhislist(string StartDate,string EndDate)
    {
      try
      {
        if (StartDate == null && EndDate == null)
        {
          //預設今日
          StartDate = DateTime.Now.ToString("yyyy-MM-dd");
          EndDate = DateTime.Now.ToString("yyyy-MM-dd");

          //return View();
        }

        ViewBag.StartDate = StartDate;
        ViewBag.EndDate = EndDate;

        var data = getHisData(StartDate, EndDate);


        ViewBag.count = data.Count;
        ViewData["LRTIAlert"] = data;
      }
      catch (Exception ex)
      {
        Logger.Log(NLog.LogLevel.Error, ex.Message);
      }
      
      
      return View();
    }
    

    [HttpPost]
    public ActionResult ExportExcel(string StartDate, string EndDate)
    {
      // 將資料寫入串流
      MemoryStream files = new MemoryStream();

      try
      {
        string sSaveFilePath = @"d:\temp\" + "AlertLRTI_" + Guid.NewGuid().ToString() + ".xlsx";
        
        using (FileStream fs = System.IO.File.OpenRead(@"c:\test.xls"))
        {
          fs.CopyTo(files);
        }

        //workSpase.Write(files);
        files.Close();
      }
      catch (Exception ex)
      {
        Logger.Log(NLog.LogLevel.Error, ex.Message);
      }
   

      return this.File(files.ToArray(), "application/vnd.ms-excel", "Download.xlsx"); ;
    }

   
    public ActionResult down(string StartDate, string EndDate)
    {
      try
      {
        var data = getHisData(StartDate, EndDate);

        ViewBag.StartDate = StartDate;
        ViewBag.EndDate = EndDate;


        //產生檔案路徑
        string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
        //建立資料夾
        Directory.CreateDirectory(sTempPath);
        string sSaveFilePath = Path.Combine(sTempPath, "ExportAlertLRTI_" + Guid.NewGuid().ToString() + ".xlsx");

        DataTable dt = new DataTable();
        dt.Columns.Add("RecTime");
        dt.Columns.Add("country");
        dt.Columns.Add("town");
        dt.Columns.Add("village");
        dt.Columns.Add("status");
        dt.Columns.Add("HOUR1");
        dt.Columns.Add("HOUR2");
        dt.Columns.Add("HOUR3");
        dt.Columns.Add("LRTI");
        dt.Columns.Add("ELRTI");
        dt.Columns.Add("RT");
        dt.Columns.Add("STID");
        dt.Columns.Add("STNAME");

        foreach (var item in data)
        {
          DataRow NewRow = dt.NewRow();
          NewRow["RecTime"] = item.RecTime;
          NewRow["country"] = item.country;
          NewRow["town"] = item.town;
          NewRow["village"] = item.village;
          NewRow["status"] = item.status;
          NewRow["HOUR1"] = item.HOUR1;
          NewRow["HOUR2"] = item.HOUR2;
          NewRow["HOUR3"] = item.HOUR3;
          NewRow["LRTI"] = item.LRTI;
          NewRow["ELRTI"] = item.ELRTI;
          NewRow["RT"] = item.RT;
          NewRow["STID"] = item.STID;
          NewRow["STNAME"] = item.STNAME;

          dt.Rows.Add(NewRow);
        }

        DataExport de = new DataExport();
        de.ExportBigDataToExcel(sSaveFilePath, dt);

        if (System.IO.File.Exists(sSaveFilePath))
        {
          string filename = string.Format("AlertLRTI_{0}_{1}.xlsx", StartDate, EndDate);
          filename = filename.Replace("-", "");
          //讀成串流
          Stream iStream = new FileStream(sSaveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);


          //回傳出檔案
          return File(iStream, "application/vnd.ms-excel", filename);
        }

      }
      catch (Exception ex)
      {
        Logger.Log(NLog.LogLevel.Error, ex.Message);
      }
      
      return View("warnhislist");
     
    }

    public ActionResult warndatatables()
    {
      try
      {
        //var data = db.Query(@" select * from RunTimeRainData ");

        var AlertUpdateTm = dbDapper.Query(@" select * from LRTIAlertMail where type = 'altm' ");



        ViewBag.nowdate = "";
        if (AlertUpdateTm.Count > 0)
        {
          ViewBag.nowdate = AlertUpdateTm[0].value;
        }


        //新增
        var dataI = dbDapper.Query(@" select * from LRTIAlert where status = 'I' order by country,town ");
        //持續
        var dataC = dbDapper.Query(@" select * from LRTIAlert where status = 'C' order by country,town ");
        //解除
        var dataD = dbDapper.Query(@" select * from LRTIAlert where status = 'D' order by country,town ");


        List<dynamic> data = new List<dynamic>();
        data.AddRange(dataI);
        data.AddRange(dataC);
        data.AddRange(dataD);

        foreach (var item in data)
        {
          if (item.status == "I") item.status = M10Const.AlertStatus.I;
          if (item.status == "C") item.status = M10Const.AlertStatus.C;
          if (item.status == "O") item.status = M10Const.AlertStatus.O;
          if (item.status == "D") item.status = M10Const.AlertStatus.D;
        }


        ViewBag.count = data.Count;
        ViewData["LRTIAlert"] = data;
      }
      catch (Exception ex)
      {
        Logger.Log(NLog.LogLevel.Error, ex.Message);
      }

      return View();
    }


    private List<dynamic> getHisData(string StartDate, string EndDate)
    {
      List<dynamic> ResultList = new List<dynamic>();

      DateTime dtStart;
      DateTime dtEnd;

      if (StartDate == null || EndDate == null)
      {
        dtStart = DateTime.Now;
        dtEnd = DateTime.Now;
      }
      else
      {
        string[] aSdt = StartDate.Split('-');
        string[] aEdt = EndDate.Split('-');

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), 0, 0, 1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), 23, 59, 59);
      }

      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");
      string ssql = @"  select LRTIAlertHis.*,StationData.STNAME from LRTIAlertHis
                        left join StationData on LRTIAlertHis.STID = StationData.STID
                        where 1=1
                        and RecTime between '{0}' and '{1}'
                        order by RecTime,country,town  ";

      var data = dbDapper.Query(string.Format(ssql, sStartDate, sEndDate));
      ResultList.AddRange(data);

      foreach (var item in ResultList)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = M10Const.AlertStatus.I;
        if (item.status == "C") item.status = M10Const.AlertStatus.C;
        if (item.status == "O") item.status = M10Const.AlertStatus.O;
        if (item.status == "D") item.status = M10Const.AlertStatus.D;


        //處理ELRTI取至小數第二位
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Round(dELRTI, 2).ToString();
        }

        decimal dRT = 0;
        if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
        {
          item.RT = Math.Round(dRT, 2).ToString();
        }



      }

      return ResultList;
    }
  }

  public class testclass {
    public string StartDate;
    public string EndDate;


  }
}