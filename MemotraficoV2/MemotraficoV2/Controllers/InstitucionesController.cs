using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.Filters;
using System.Web.Security;


namespace MemotraficoV2.Controllers
{
    [Authorize, Acceso]
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
        public JsonResult Create(Institucion i)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Institucion.AddObject(i);

                db.SaveChanges();

                return Json(new
                {
                    result = true,
                    dir = "/Instituciones",
                    msj = "La institucion se a registrado Correctamente"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Instituciones/Create",
                    msj = "El registro no se pudo completar, Intenta nuevamente"
                });
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
        public JsonResult Edit(Institucion inst)
        {
            try
            {
                var i = inst.Editar();

                return Json(new
                {
                    result = true,
                    dir = "/Instituciones",
                    msj = "Se actualizo correctamente el registro"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Instituciones/Edit/" + inst.IdInstitucion + "",
                    msj = "El registro no se pudo actualizar, Intenta nuevamente"
                });
            }
        }

        [HttpPost]
        public JsonResult Elimina(int id = 0)
        {
            try
            {
                SASEntities db = new SASEntities();
                var inst = db.Institucion.FirstOrDefault(i => i.IdInstitucion == id);
                db.DeleteObject(inst);
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
