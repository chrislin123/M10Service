using M10Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace M10Api.Class
{
  public class BaseApiController : ApiController
  {
    protected DBContext db = new DBContext();
  }
}