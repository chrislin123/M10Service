using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M10Api.Controllers
{
  public class MapController : Controller
  {
    // GET: Map
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult map()
    {
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