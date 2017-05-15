using M10Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace M10Api.Class
{
  public class BaseController : Controller
  {
    protected DBContext db = new DBContext();
  }
}