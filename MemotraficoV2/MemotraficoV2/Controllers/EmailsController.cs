using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    public class EmailsController : Controller
    {
        // GET: Emails
        public ActionResult Index()
        {
            return View();
        }

        // GET: Emails/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Emails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Emails/Create
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

        // GET: Emails/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Emails/Edit/5
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

        // GET: Emails/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Emails/Delete/5
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
