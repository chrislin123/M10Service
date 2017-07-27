using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DhoeMvc.Controllers
{
    public class PhotoController : Controller
    {
        // GET: Photo
        public ActionResult Photo()
        {
            return View();
        }
    }
}