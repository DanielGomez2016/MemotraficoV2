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
    public class ProcedenciaController : Controller
    {
        // GET: Procedencia
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Procedencia> p = db.Procedencia.ToList();
            List<Procedencia> plist = new List<Procedencia>();
            foreach(var x in p)
            {
                x.NombreLocalidad = db.Localidades.FirstOrDefault(i => i.IdLocalidad == x.Localidad).Nombre.ToString();
                x.NombreMunicipio = db.Municipios.FirstOrDefault(i => i.IdMunicipio == x.Municipio).Nombre.ToString();
                plist.Add(x);
            }
            return View(plist);
        }

        public ActionResult Create()
        {
            SASEntities db = new SASEntities();
            ViewBag.TipoProcedencia = db.TipoProcedencia.Select(i => new { id = i.IdTipoProcedencia, nombre = i.TipoProcedencia1 }).ToList();
            ViewBag.Municipios = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult Create(Procedencia p)
        {
            try
            {
                p.Crear();

                return Json(new
                {
                    result = true,
                    dir = "/Procedencia",
                    msj = "La procedencia se a registrado Correctamente"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Procedencias/Create",
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
            }
        }

        public ActionResult Edit(int id)
        {
            SASEntities db = new SASEntities();

            Procedencia p = db.Procedencia.FirstOrDefault(i => i.IdProcedencia == id);

            ViewBag.TipoProcedencia = db.TipoProcedencia.Select(i => new { id = i.IdTipoProcedencia, nombre = i.TipoProcedencia1 }).ToList();
            ViewBag.Municipios = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            ViewBag.Localidades = db.Localidades.Where(i => i.IdMunicipioFk == p.Municipio).Select(i => new { id = i.IdLocalidad, nombre = i.Nombre });

            return View(p);
        }

        [HttpPost]
        public JsonResult Edit(Procedencia p)
        {
            try
            {
                p.Editar();

                return Json(new
                {
                    result = true,
                    dir = "/Procedencia",
                    msj = "El registro se actualizo correctamente"
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Procedencias/Edit" + p.IdProcedencia,
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
            }
        }
    }
}