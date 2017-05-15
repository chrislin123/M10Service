using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M10Api.Class;
using Dapper;

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
      //var data = 
      //var data = (from News in db.HotNews
      //            where News.chCancel == "N"
      //            orderby News.intNewsID descending
      //            select News).Take(10);

      //var data = db.Query(@" select a.*,b.lat,b.lon from StationData a 
      //  left join runtimeraindata b on a.stid = b.stid 

      //  "
      //  );



      

      var data = db.Query(@" select * from RunTimeRainData ");
      //List<Models.RunTimeRainData> rdata = new List<Models.RunTimeRainData>();


      //foreach (var item in data)
      //{
      //  Models.RunTimeRainData rtr = new Models.RunTimeRainData();
      //  rtr.STID = item.STID;


      //  rdata.Add(rtr);
      
      //}

      ViewBag.nowdate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
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

    public ActionResult warndatatables()
    {
     
      var data = db.Query(@" select * from RunTimeRainData ");
      //List<Models.RunTimeRainData> rdata = new List<Models.RunTimeRainData>();


      //foreach (var item in data)
      //{
      //  Models.RunTimeRainData rtr = new Models.RunTimeRainData();
      //  rtr.STID = item.STID;


      //  rdata.Add(rtr);

      //}

      ViewBag.nowdate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
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
}