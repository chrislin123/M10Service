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

namespace M10Api.Controllers
{
  public class RainController : BaseController
  {
    // GET: Rain
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult QueryRain()
    {
      var AlertUpdateTm = db.ExecuteScale(@" select value from LRTIAlertMail where type = 'altm' ");
      ViewBag.forecastdate = AlertUpdateTm == null ? "" : AlertUpdateTm.ToString();

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
        //if (item.status == "I") item.status = "新增";
        //if (item.status == "C") item.status = "持續";
        //if (item.status == "D") item.status = "刪除";
        if (item.status == "I") item.status = Constants.AlertStatus.I;
        if (item.status == "C") item.status = Constants.AlertStatus.C;
        if (item.status == "O") item.status = Constants.AlertStatus.O;
        if (item.status == "D") item.status = Constants.AlertStatus.D;

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

      return View();
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