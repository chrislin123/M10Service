using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;
using M10.lib;

namespace DhoeMvc.Controllers
{
  public class MembersController : BaseController
  {
    // GET: Students詹錢登教授研究室
    public ActionResult Students()
    { 
      ssql = @" select * from students where datatype = '{0}' order by kind desc ";
      List<Students> StudentList = dbDapper.Query<Students>(string.Format(ssql, DhoeConst.StudentType.CurrSt));
      List<string> KindList = StudentList.Select(x => x.kind).Distinct().ToList();      
      ViewData["DataMList"] = KindList;
      ViewData["DataDList"] = StudentList;

      StudentList = dbDapper.Query<Students>(string.Format(ssql, DhoeConst.StudentType.HisPhd));
      KindList = StudentList.Select(x => x.kind).Distinct().ToList();
      ViewData["PhdMList"] = KindList;
      ViewData["PhdDList"] = StudentList;
      
      StudentList = dbDapper.Query<Students>(string.Format(ssql, DhoeConst.StudentType.HisMas));
      KindList = StudentList.Select(x => x.kind).Distinct().ToList();
      ViewData["MasMList"] = KindList;
      ViewData["MasDList"] = StudentList;

      return View();
    }

    // GET: StudentDetail 
    public ActionResult StudentDetail(string name)
    {
      ViewBag.name = name;

      return View();
    }

    // GET: CenterMember水土保持生態工程研究中心
    public ActionResult CenterMember()
    {
      
      List<CenterMember> MemberList = dbDapper.Query<CenterMember>(@"select * from CenterMember");

      ViewData["MemberList"] = MemberList;

      List<CenterProject> ProjectList = dbDapper.Query<CenterProject>(@"select * from CenterProject");

      ViewData["ProjectList"] = ProjectList;

      return View();
    }
  }
}