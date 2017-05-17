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
      var AlertUpdateTm = db.Query(@" select * from LRTIAlertMail where type = 'altm' ");



      ViewBag.nowdate = "";
      if (AlertUpdateTm.Count > 0)
      {
        ViewBag.nowdate = AlertUpdateTm[0].value;
      }

      string ssql = @" select * from LRTIAlert where status = '{0}' order by country,town ";
      //新增
      var dataI = db.Query(string.Format(ssql, "I"));
      //持續
      var dataC = db.Query(string.Format(ssql, "C"));
      //解除
      var dataD = db.Query(string.Format(ssql, "D"));


      List<dynamic> data = new List<dynamic>();
      data.AddRange(dataI);
      data.AddRange(dataC);
      data.AddRange(dataD);

      foreach (var item in data)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = "新增";
        if (item.status == "C") item.status = "持續";
        if (item.status == "D") item.status = "刪除";

        
        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString( item.ELRTI), out dELRTI))
        { 
          item.ELRTI = Math.Floor(dELRTI).ToString();
        } 
        
      }


      ViewBag.count = data.Count;
      ViewData["LRTIAlert"] = data;

      return View();
    }

    
    public ActionResult warnhislist(string StartDate,string EndDate)
    {


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

        dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]),0,0,1);
        dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]),23,59,59);
      }


    
      string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
      string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");
      string ssql = @" select * from LRTIAlertHis where status = '{0}' 
                      and RecTime between '{1}' and '{2}'
                      order by country,town ";
      //新增
      var dataI = db.Query(string.Format(ssql, "I", sStartDate, sEndDate));
      //持續
      var dataC = db.Query(string.Format(ssql, "C", sStartDate, sEndDate));
      //解除
      var dataD = db.Query(string.Format(ssql, "D", sStartDate, sEndDate));


      List<dynamic> data = new List<dynamic>();
      data.AddRange(dataI);
      data.AddRange(dataC);
      data.AddRange(dataD);

      foreach (var item in data)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = "新增";
        if (item.status == "C") item.status = "持續";
        if (item.status == "D") item.status = "刪除";


        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Floor(dELRTI).ToString();
        }

      }


      ViewBag.count = data.Count;
      ViewData["LRTIAlert"] = data;
      
      return View();
    }

    [HttpPost]
    public ActionResult warnhislistq(testclass tc)
    {
      //var AlertUpdateTm = db.Query(@" select * from LRTIAlertMail where type = 'altm' ");



      //ViewBag.nowdate = "";
      //if (AlertUpdateTm.Count > 0)
      //{
      //  ViewBag.nowdate = AlertUpdateTm[0].value;
      //}

      string ssql = @" select * from LRTIAlert where status = '{0}' order by country,town ";
      //新增
      var dataI = db.Query(string.Format(ssql, "I"));
      //持續
      var dataC = db.Query(string.Format(ssql, "C"));
      //解除
      var dataD = db.Query(string.Format(ssql, "D"));


      List<dynamic> data = new List<dynamic>();
      data.AddRange(dataI);
      data.AddRange(dataC);
      data.AddRange(dataD);

      foreach (var item in data)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = "新增";
        if (item.status == "C") item.status = "持續";
        if (item.status == "D") item.status = "刪除";


        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Floor(dELRTI).ToString();
        }

      }


      ViewBag.count = data.Count;
      ViewData["LRTIAlert"] = data;

      return View("warnhislist");
    }

    public ActionResult warndatatables()
    {

      //var data = db.Query(@" select * from RunTimeRainData ");

      var AlertUpdateTm = db.Query(@" select * from LRTIAlertMail where type = 'altm' ");
      


      ViewBag.nowdate = "";
      if (AlertUpdateTm.Count > 0)
      {
        ViewBag.nowdate = AlertUpdateTm[0].value;
      }
      
      
      //新增
      var dataI = db.Query(@" select * from LRTIAlert where status = 'I' order by country,town ");
      //持續
      var dataC = db.Query(@" select * from LRTIAlert where status = 'C' order by country,town ");
      //解除
      var dataD = db.Query(@" select * from LRTIAlert where status = 'D' order by country,town ");


      List<dynamic> data = new List<dynamic>();
      data.AddRange(dataI);
      data.AddRange(dataC);
      data.AddRange(dataD);

      foreach (var item in data)
      {
        if (item.status == "I") item.status = "新增";
        if (item.status == "C") item.status = "持續";
        if (item.status == "D") item.status = "刪除";
      }


      ViewBag.count = data.Count;
      ViewData["LRTIAlert"] = data;

      //var rdata;
      //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      //{
      //  rdata = cn.Query<Models.RunTimeRainData>(sql);
      //  return cn.Query(sql).ToList();
      //}


      //rdata = data.ToList<Models.RunTimeRainData>();

      return View();
    }
  }

  public class testclass {
    public string StartDate;
    public string EndDate;


  }
}