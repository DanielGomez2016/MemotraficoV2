using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.Filters;

namespace MemotraficoV2.Controllers
{
    public class LocalidadesController : Controller
    {
        [Authorize, Acceso]
        // GET: Localidades
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();

            List<Localidades> e = db.Localidades.ToList();
            return View(e);
        }

        public ActionResult Create()
        {
            SASEntities db = new SASEntities();

            ViewBag.Municipios = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            return View();
        }

        // POST: Localidades/Create
        [HttpPost]
        public JsonResult Create(Localidades l)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Localidades.AddObject(l);

                db.SaveChanges();

                return Json(new
                {
                    result = true,
                    dir = "/Localidades",
                    msj = "La localidad se a registrado Correctamente"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Localidades/Create",
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
            }
        }

        public ActionResult Edit(int id)
        {
            SASEntities db = new SASEntities();
            Localidades l = db.Localidades.FirstOrDefault(i => i.IdLocalidad == id);

            ViewBag.Municipios = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();

            return View(l);
        }

        // POST: Departamentos/Edit/5
        [HttpPost]
        public JsonResult Edit(Localidades l)
        {
            try
            {
                l.Editar();

                return Json(new
                {
                    result = true,
                    dir = "/Localidades",
                    msj = "Se actualizo correctamente el registro"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Localidades/Edit/"+l.IdLocalidad+"",
                    msj = "El registro no se pudo actualizar, Intenta nuevamente"
                });
            }
        }
    }
}