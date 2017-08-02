using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;

namespace DhoeMvc.Controllers
{
  public class NewsController : BaseController
  {


    public ActionResult News()
    {

      //List<dynamic> NewsmList = dbDapper.Query(@"select * from newsm
      //                  order by date desc");

      List<Newsm> NewsmList = dbDapper.Query<Newsm>(@"select * from newsm
                        order by date desc");


      ViewData["NewsmList"] = NewsmList;



      return View();
    }


  }
}
