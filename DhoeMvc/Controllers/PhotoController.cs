using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;

namespace DhoeMvc.Controllers
{
  public class PhotoController : BaseController
  {
    // GET: Photo
    public ActionResult Photo()
    { 
      List<AlbumM> DataList = dbDapper.Query<AlbumM>(@"select * from AlbumM
                        order by sort desc");

      ViewData["DataList"] = DataList;

      return View();
    }
  }
}