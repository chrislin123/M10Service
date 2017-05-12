using M10Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M10Api.Class
{
  public class BaseApiController
  {
    protected DBContext db = new DBContext();
  }
}