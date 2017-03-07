using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;

namespace MemotraficoV2.Controllers
{
    public class InstitucionesController : Controller
    {
        // GET: Instituciones
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Institucion> i = db.Institucion.ToList();
            return View(i);
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
        public ActionResult Create(Institucion inst)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Institucion.AddObject(inst);
                db.SaveChanges();

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
            SASEntities db = new SASEntities();
            Institucion ins = db.Institucion.FirstOrDefault(i => i.IdInstitucion == id);

            return View(ins);
        }

        // POST: Instituciones/Edit/5
        [HttpPost]
        public ActionResult Edit(Institucion inst)
        {
            try
            {
                var i = inst.Editar();
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
