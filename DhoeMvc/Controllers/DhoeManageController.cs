using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;
using M10.lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DhoeMvc.Models.DhoeManageModels;

namespace DhoeMvc.Controllers
{
  public class DhoeManageController : BaseController
  {
    // GET: DhoeManage
    public ActionResult Index()
    {
      return View();
    }


    public ActionResult ManageStudent()
    {
      return View();
    }

    public ActionResult getstudentdata(string type)
    { 
      ssql = " select * from students where datatype = '{0}' order by kind desc ";
      ssql = string.Format(ssql, type);
      List<Students> StudentList = dbDapper.Query<Students>(ssql);

      JArray ja = new JArray();
      foreach (Students LoopItem in StudentList)
      {
        var itemobj = new JObject
        {
          {"name",LoopItem.name },
          {"kind",LoopItem.kind },
          {"datatype",LoopItem.datatype },
          {"no",LoopItem.no },
        };

        ja.Add(itemobj);
      }

      //JObject jo = new JObject();
      //jo.Add("total", StudentList.Count());
      //jo.Add("rows", ja);

      return Content(JsonConvert.SerializeObject(ja), "application/json");
    }

    [HttpPost]
    public ActionResult ManageStudentAdd(Students Data)
    {

      //Students st = new Students();
      //st.name = Data.Name;
      //st.datatype = Data.DataType;
      //st.kind = Data.Kind;
      dbDapper.Insert(Data);


      JObject joResult = new JObject();
      joResult.Add("msg", "ok");
      
      return Content(JsonConvert.SerializeObject(joResult), "application/json");
    }

    [HttpPost]
    public ActionResult ManageStudentMod(Students Data)
    {
      JObject joResult = new JObject();

      try
      {
        //Students st = new Students();
        //st.name = Data.Name;
        //st.datatype = Data.DataType;
        //st.kind = Data.Kind;
        //st.no = Data.no;
        dbDapper.Update(Data);
        
        joResult.Add("msg", "ok");
      }
      catch (Exception ex)
      {
        joResult.Add("msg", ex.Message);
      }

      return Content(JsonConvert.SerializeObject(joResult), "application/json");
    }


    [HttpPost]
    public ActionResult ManageStudentDel(string no)
    {
      JObject joResult = new JObject();
      
      try
      {
        ssql = " delete students where no = '{0}' ";
        ssql = string.Format(ssql, no);

        dbDapper.Execute(ssql);

        joResult.Add("msg", "ok");
      }
      catch (Exception ex)
      {
        joResult.Add("msg", ex.Message);
      }

      return Content(JsonConvert.SerializeObject(joResult), "application/json");
    }

    [HttpGet]
    public ActionResult ManageStudentGetKindData(string kind)
    {
      ssql = " select * from dhoeset where settype = 'studentkind' and setkind = '{0}'  order by setseq ";
      ssql = string.Format(ssql, kind);
      List<DhoeSet> SetList = dbDapper.Query<DhoeSet>(ssql);

      var data = from u in SetList
                 select new {
                   text = u.setvalue,
                   value = u.setvalue
                 };

      return Content(JsonConvert.SerializeObject(data), "application/json");


      //JArray ja = new JArray();
      //foreach (DhoeSet item in SetList)
      //{
      //  JObject jo = new JObject();
      //  jo.Add("text", item.setvalue);
      //  jo.Add("value", item.setvalue);
      //  ja.Add(jo);
      //}

      //return Content(JsonConvert.SerializeObject(ja), "application/json");
    }




  }



  
}