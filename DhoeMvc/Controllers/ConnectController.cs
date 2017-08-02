using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;

namespace DhoeMvc.Controllers
{
  public class ConnectController : BaseController
  {

    public ActionResult Connect()
    {
      List<Connect> DataList = dbDapper.Query<Connect>(@"select * from Connect");
      
      ViewData["DataList"] = DataList;

      return View();
    }


  }
}
