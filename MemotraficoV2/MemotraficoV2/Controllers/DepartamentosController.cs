using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;

namespace MemotraficoV2.Controllers
{
    public class DepartamentosController : Controller
    {
        // GET: Departamentos
        public ActionResult Index(string id)
        {
            var x = Convert.ToInt32(id);
            SASEntities db = new SASEntities();
            List<Departamento> d = db.Departamento.Where(i => i.IdInstitucionFk == x).ToList();
            return View(d);
        }

        // GET: Departamentos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Departamentos/Create
        public ActionResult Create(string id)
        {
            var x = Convert.ToInt32(id);
            SASEntities db = new SASEntities();
            if (x == 0)
            {
                ViewBag.Institucion = db.Institucion.Select(i => new { id = i.IdInstitucion, nombre = i.Siglas }).ToList();
            }
            else { 
            ViewBag.Institucion = db.Institucion.Where(i => i.IdInstitucion == x).Select(i => new { id = i.IdInstitucion, nombre = i.Siglas }).ToList();
            }
            return View();
        }

        // POST: Departamentos/Create
        [HttpPost]
        public ActionResult Create(Departamento depto)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Departamento.AddObject(depto);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Departamentos/Edit/5
        public ActionResult Edit(int id)
        {
            SASEntities db = new SASEntities();
            Departamento dep = db.Departamento.FirstOrDefault(i => i.IdDepartamento == id);

            return View(dep);
        }

        // POST: Departamentos/Edit/5
        [HttpPost]
        public ActionResult Edit(Departamento depto)
        {
            try
            {
                var i = depto.Editar();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult Elimina(int id = 0)
        {
            try
            {
                SASEntities db = new SASEntities();
                var depto = db.Departamento.FirstOrDefault(i => i.IdDepartamento == id);
                db.DeleteObject(depto);
                db.SaveChanges();

                return Json(new
                {
                    result = true
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
