using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.Filters;

namespace MemotraficoV2.Controllers
{
    [Authorize, Acceso]
    public class MunicipiosController : Controller
    {
        // GET: Municipios
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Municipios> m = db.Municipios.ToList();
            return View(m);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Municipios/Create
        [HttpPost]
        public JsonResult Create(Municipios m)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Municipios.AddObject(m);

                db.SaveChanges();

                return Json(new
                {
                    result = true,
                    dir = "/Municipios",
                    msj = "El municipio se ha registrado Correctamente"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Municipios/Create",
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
            }
        }

        public ActionResult Edit(int id)
        {
            SASEntities db = new SASEntities();

            Municipios m = db.Municipios.FirstOrDefault(i => i.IdMunicipio == id);

            return View(m);
        }

        // POST: Municipios/Edit/5
        [HttpPost]
        public JsonResult Edit(Municipios m)
        {
            try
            {
                m.Editar();

                return Json(new
                {
                    result = true,
                    dir = "/Municipios",
                    msj = "Se actualizo correctamente el registro"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Municipios/Edit/" + m.IdMunicipio + "",
                    msj = "El registro no se pudo actualizar, Intenta nuevamente"
                });
            }
        }
    }
}