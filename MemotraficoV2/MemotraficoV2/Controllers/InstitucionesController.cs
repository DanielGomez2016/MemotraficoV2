using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    public class InstitucionesController : Controller
    {
        // GET: Instituciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: Instituciones/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Instituciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Instituciones/Create
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

        // GET: Instituciones/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Instituciones/Edit/5
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

        // GET: Instituciones/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Instituciones/Delete/5
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
