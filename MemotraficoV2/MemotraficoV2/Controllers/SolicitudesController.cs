using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.ViewModels;
using MemotraficoV2.Models.Colecciones;

namespace MemotraficoV2.Controllers
{
    public class SolicitudesController : Controller
    {
        // GET: Solicitudes
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Escuela> e = db.Escuela.ToList();

            SolicitudesEscuelasViewModel s = new SolicitudesEscuelasViewModel();
            List<SolicitudesEscuelasViewModel> se = new List<SolicitudesEscuelasViewModel>();

            foreach(var r in e)
            {
                s.idEscuela = r.IdEscuela;
                s.escuela = r.Clave + " " + r.Nombre;
                s.director = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == r.IdEscuela).Nombre.ToString();
                s.localidad = r.Localidades.Nombre + ", " + r.Municipios.Nombre;
                s.Solicitudes = db.Solicitudes.Where(j => j.IdEscuelaFk == r.IdEscuela).Count();
                se.Add(s);
            }
            return View(se);
        }

        public JsonResult Autocomplete(string term)
        {
            return Json(Solicitudes.AutocompleteEsc(term), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registro()
        {
            SASEntities db = new SASEntities();

            ViewBag.Estatus = db.Estatus.Select(i => new { id = i.IdEstatus, nombre = i.Estatus1}).ToList();
            ViewBag.TipoAsunto = db.TipoAsunto.Select(i => new { id = i.IdTipoAsunto, nombre = i.TipoAsunto1 }).ToList();
            ViewBag.TipoProcedencia = db.TipoProcedencia.Select(i => new { id = i.IdTipoProcedencia, nombre = i.TipoProcedencia1 }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult Registro(Solicitudes solicitud)
        {
            try
            {
                SASEntities db = new SASEntities();
                int v = solicitud.Crear();
                Canalizacion c = new Canalizacion();
                c.IdInstitucionFk = Usuarios.GetInstitucion();
                c.IdDepartamentoFk = Usuarios.GetDepto();
                c.Validacion = Convert.ToBoolean(ListaValidaciones.NO_VALIDACION);
                c.IdSolicitudFk = v;

                int v2 = c.Crear();
                DetalleCanalizacion dc = new DetalleCanalizacion();
                dc.IdCanalizarFk = v2;
                dc.FechaCanalizar = DateTime.Now;
                dc.Comentario = ListaComentarios.INICIADA;
                dc.IdUsuarioFk = Usuarios.GetUsuario();
                dc.Departamento = Usuarios.GetDepto();
                dc.Instituto = Usuarios.GetInstitucion();
                var v3 = dc.Crear();

                return Json(new
                {
                    result = true,
                    casee = new {
                        iddetcanalizacion = v3,
                        namevar = "#iddetcanalizacion"
                    },
                    dir = "" ,
                    msj = "La solicitud se a registrado Correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/Registro/",
                    msj = "La Solicitud no se a podido registrar, intente nuevamente"
                });

            }
        }

        public ActionResult Escuela(int esc)
        {
            return View();
        }

        public JsonResult Documento(int doc,HttpPostedFileBase file)
        {
            return Json(false);
        }
    }
}