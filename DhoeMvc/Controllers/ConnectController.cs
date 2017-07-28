using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;

namespace DhoeMvc.Controllers
{
  public class ConnectController : BaseController
  {

    public ActionResult Connect()
    {

      List<dynamic> DataList = dbDapper.Query(@"select * from Connect");

      //List<Newsm> NewsmList = dbDapper.Query<Newsm>(@"select * from newsm
      //                  order by date desc");

      int iRowIndex = 0;
      foreach (var item in DataList)
      {
        iRowIndex++;

        item.type = "";
        if (iRowIndex % 2 == 1)
        {
          item.type = "1";
        }
      }


      ViewData["DataList"] = DataList;

      return View();
    }


  }
}
