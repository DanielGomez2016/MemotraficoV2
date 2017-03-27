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
    public class EscuelasController : Controller
    {    
        // GET: Escuelas
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Escuela> e = db.Escuela.ToList();
            List<Escuela> ec = new List<Escuela>();

            foreach(var x in e)
            {
                x.NombreDirector = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == x.IdEscuela).Nombre.ToString();
                x.EmailDirector = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == x.IdEscuela).Email.ToString();
                x.Telefono = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == x.IdEscuela).Telefono.ToString();
                x.Celular = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == x.IdEscuela).Celular.ToString();

                ec.Add(x);
            }

            return View(ec);
        }

        public ActionResult Create()
        {
            SASEntities db = new SASEntities();
            ViewBag.Municipio = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            ViewBag.NivelEducativo = db.NivelEducativo.Select(i => new { id = i.IdNivelEducativo, nombre = i.Nivel }).ToList();
            return View();
        }

        // POST: Escuelas/Create
        [HttpPost]
        public JsonResult Create(Escuela escuela)
        {
            try
            {
                SASEntities db = new SASEntities();
                var e = escuela.Crear();

                Contacto c = new Contacto();
                c.Nombre = escuela.NombreDirector;
                c.Email = escuela.EmailDirector;
                c.Telefono = escuela.Telefono;
                c.Celular = escuela.Celular;
                c.IdEscuelaFk = e;
                db.Contacto.AddObject(c); 

                db.SaveChanges();

                return Json(new
                {
                    result = true,
                    dir = "/Escuelas",
                    msj = "La esceula se a registrado Correctamente"
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

            Escuela e = db.Escuela.FirstOrDefault(i => i.IdEscuela == id);

            e.NombreDirector = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == e.IdEscuela).Nombre.ToString();
            e.EmailDirector = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == e.IdEscuela).Email.ToString();
            e.Telefono = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == e.IdEscuela).Telefono.ToString();
            e.Celular = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == e.IdEscuela).Celular.ToString();

            ViewBag.Municipio = db.Municipios.Select(i => new { id = i.IdMunicipio, nombre = i.Nombre }).ToList();
            ViewBag.Localidad = db.Localidades.Where(i => i.IdLocalidad == e.IdMunicipioFk).Select(i => new { id = i.IdLocalidad, nombre = i.Nombre }).ToList();
            ViewBag.NivelEducativo = db.NivelEducativo.Select(i => new { id = i.IdNivelEducativo, nombre = i.Nivel }).ToList();

            return View(e);
        }

        // POST: Escuelas/Edit/5
        [HttpPost]
        public JsonResult Edit(Escuela e)
        {
            try
            {
                var es = e.Editar();

                SASEntities db = new SASEntities();
                Contacto c = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == es);
                c.Editar();


                return Json(new
                {
                    result = true,
                    dir = "/Escuelas",
                    msj = "Se actualizo correctamente el registro"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Escuela/Create",
                    msj = "No se pudo actualizar el registro"
                });
            }
        }
    }
}