using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;

namespace DhoeMvc.Controllers
{
    public class NewsController : BaseController
    {


    public ActionResult News()
    {
      var AlertUpdateTm = dbDapper.ExecuteScale(@" select value from LRTIAlertMail where type = 'altm' ");
      ViewBag.forecastdate = AlertUpdateTm == null ? "" : AlertUpdateTm.ToString();

      //string ssql = @" select * from LRTIAlert where status = '{0}' order by country,town ";
      ////新增
      //var dataI = dbDapper.Query(string.Format(ssql, "I"));
      ////持續
      //var dataC = dbDapper.Query(string.Format(ssql, "C"));
      ////解除
      //var dataD = dbDapper.Query(string.Format(ssql, "D"));


      //List<dynamic> data = new List<dynamic>();
      //data.AddRange(dataI);
      //data.AddRange(dataC);
      //data.AddRange(dataD);

      //foreach (var item in data)
      //{
      //  //處理狀態改中文顯示
      //  //if (item.status == "I") item.status = "新增";
      //  //if (item.status == "C") item.status = "持續";
      //  //if (item.status == "D") item.status = "刪除";
      //  if (item.status == "I") item.status = Constants.AlertStatus.I;
      //  if (item.status == "C") item.status = Constants.AlertStatus.C;
      //  if (item.status == "O") item.status = Constants.AlertStatus.O;
      //  if (item.status == "D") item.status = Constants.AlertStatus.D;

      //  //處理ELRTI取至小數第二位
      //  decimal dELRTI = 0;
      //  if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
      //  {
      //    item.ELRTI = Math.Round(dELRTI, 2).ToString();
      //  }

      //  decimal dRT = 0;
      //  if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
      //  {
      //    item.RT = Math.Round(dRT, 2).ToString();
      //  }

      //}


      //ViewBag.count = data.Count;
      //ViewData["LRTIAlert"] = data;

      return View();
    }


    // GET: News
    public ActionResult Index()
        {
            return View();
        }

        // GET: News/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: News/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: News/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: News/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: News/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
