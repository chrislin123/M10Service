using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;

namespace DhoeMvc.Controllers
{
    public class ConnectController : BaseController
    {

    public ActionResult Connect()
    {
     
      return View();
    }


    // GET: Connect
    public ActionResult Index()
        {
            return View();
        }

        // GET: Connect/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Connect/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Connect/Create
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

        // GET: Connect/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Connect/Edit/5
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

        // GET: Connect/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Connect/Delete/5
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
