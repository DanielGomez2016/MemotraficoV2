using MemotraficoV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    public class BeneficiariosController : Controller
    {
        // GET: Beneficiarios
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Beneficiario> b = db.Beneficiario.ToList();
            return View(b);
        }

        public ActionResult Create()
        {
            SASEntities db = new SASEntities();
            ViewBag.Municipio = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            return View();
        }

        // POST: Beneficiarios/Create
        [HttpPost]
        public JsonResult Create(Beneficiario b)
        {
            try
            {
                SASEntities db = new SASEntities();
                var e = b.Crear();

                return Json(new
                {
                    result = true,
                    dir = "/Beneficiarios",
                    msj = "El Beneficiario se a registrado Correctamente"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
            }
        }

        public ActionResult Edit(int id)
        {
            SASEntities db = new SASEntities();

            Beneficiario e = db.Beneficiario.FirstOrDefault(i => i.IdBeneficiario == id);

            ViewBag.Municipio = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            ViewBag.Localidad = db.Localidades.Where(i => i.IdMunicipioFk == e.IdMunicipioFk).Select(i => new { id = i.IdLocalidad, nombre = i.Nombre }).ToList();

            return View(e);
        }

        // POST: Beneficiarios/Edit/5
        [HttpPost]
        public JsonResult Edit(Beneficiario b)
        {
            try
            {
                var es = b.Editar();

                return Json(new
                {
                    result = true,
                    dir = "/Beneficiarios",
                    msj = "Se actualizo correctamente el registro"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Beneficiarios/Create",
                    msj = "No se pudo actualizar el registro"
                });
            }
        }
    }
}