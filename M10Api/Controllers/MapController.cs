using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M10Api.Models;
using M10Api.Class;

namespace M10Api.Controllers
{
  public class MapController : BaseController
  {
    //protected DBContext db = new DBContext();

    // GET: Map
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult map()
    {
      var AlertUpdateTm = dbDapper.ExecuteScale(@" select value from LRTIAlertMail where type = 'altm' ");
      ViewBag.forecastdate = AlertUpdateTm == null ? "" : AlertUpdateTm.ToString();

      return View();
      //return View("mymap");
    }


    public ActionResult mymap()
    {
      return View();
      //return View("Index");
    }
  }
}