using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;


namespace DhoeMvc.Controllers
{
  public class ProfessorController : BaseController
  {
    // GET: Professor
    public ActionResult Professor()
    {
      return View();
    }

    // GET: Experience學歷經歷
    public ActionResult Experience()
    {
      return View();
    }

    // GET: Interests專業證照
    public ActionResult Interests()
    {
      return View();
    }

    // GET: Books專書
    public ActionResult Books()
    {
      
      List<Books> DataList = dbDapper.Query<Books>(@"select * from books
                        order by date desc");

      ViewData["DataList"] = DataList;

      return View();
    }

    // GET: JournalPaper期刊論文
    public ActionResult JournalPaper()
    {
      
      List<JournalPaper> DataList = dbDapper.Query<JournalPaper>(@"select * from JournalPaper
                        order by date desc");

      ViewData["DataList"] = DataList;
      return View();
    }

    // GET: ConferencePaper研討會論文
    public ActionResult ConferencePaper()
    {
      List<ConfPaper> DataList = dbDapper.Query<ConfPaper>(@"select * from ConfPaper
                        order by date desc");

      ViewData["DataList"] = DataList;
      return View();
    }

    // GET: Publications專書論文
    public ActionResult Publications()
    {
      List<Publications> DataList = dbDapper.Query<Publications>(@"select * from Publications
                        order by date desc");

      ViewData["DataList"] = DataList;
      return View();
    }

    // GET: Research研究計畫
    public ActionResult Research()
    {
      List<Research> DataList = dbDapper.Query<Research>(@"select * from Research");

      ViewData["DataList"] = DataList;
      return View();
    }    


  }
}