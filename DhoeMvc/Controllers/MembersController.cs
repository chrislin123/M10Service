using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;

namespace DhoeMvc.Controllers
{
  public class MembersController : BaseController
  {
    // GET: Students詹錢登教授研究室
    public ActionResult Students()
    {
      string sDataType = "currst";
      ssql = @" select * from students
              where datatype = '{0}'";
      List<Students> StudentList = dbDapper.Query<Students>(string.Format(ssql, sDataType));
      
      List<string> KindList = StudentList.Select(x => x.kind).Distinct().ToList();
      
      ViewData["DataMList"] = KindList;
      ViewData["DataDList"] = StudentList;

      sDataType = "phd";
      ssql = @" select * from students
              where datatype = '{0}'";
      StudentList = dbDapper.Query<Students>(string.Format(ssql, sDataType));

      KindList = StudentList.Select(x => x.kind).Distinct().ToList();

      ViewData["PhdMList"] = KindList;
      ViewData["PhdDList"] = StudentList;

      sDataType = "masdeg";
      ssql = @" select * from students
              where datatype = '{0}'";
      StudentList = dbDapper.Query<Students>(string.Format(ssql, sDataType));

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