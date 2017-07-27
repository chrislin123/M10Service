using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DhoeMvc.Controllers
{
  public class MembersController : Controller
  {
    // GET: Students詹錢登教授研究室
    public ActionResult Students()
    {
      return View();
    }

    // GET: CenterMember水土保持生態工程研究中心
    public ActionResult CenterMember()
    {
      return View();
    }
  }
}