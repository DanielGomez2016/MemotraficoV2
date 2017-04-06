using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.ViewModels;
using MemotraficoV2.Models.Colecciones;
using System.Collections;

namespace MemotraficoV2.Controllers
{
    public class SolicitudesController : Controller
    {
        // GET: Solicitudes
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            List<Escuela> e = db.Escuela.ToList();
 
            List<SolicitudesEscuelasViewModel> se = new List<SolicitudesEscuelasViewModel>();

            foreach(var r in e)
            {
                SolicitudesEscuelasViewModel s = new SolicitudesEscuelasViewModel();
                s.idEscuela = r.IdEscuela;
                s.escuela = r.Clave + " " + r.Nombre;
                s.director = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == r.IdEscuela).Nombre.ToString();
                s.localidad = r.Localidades.Nombre + ", " + r.Municipios.Nombre;
                s.telefono = "Tel: " + db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == r.IdEscuela).Telefono.ToString() + " Cel: " + db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == r.IdEscuela).Celular.ToString();
                s.correo = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == r.IdEscuela).Email.ToString();
                s.Solicitudes = db.Solicitudes.Where(j => j.IdEscuelaFk == r.IdEscuela).Count();
                s.direccion = r.Domicilio;
                s.turno = r.Turno;
                s.niveleducativo = r.NivelEducativo.Nivel;
                s.validacion = db.Validacion.Any(j => j.IdEscuelaFk == r.IdEscuela);

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
                var canalizacion = Canalizacion.Canalizar(v);

                return Json(new
                {
                    result = true,
                    modal = true,
                    idmodal = "model-documento",
                    val = canalizacion,
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

        [HttpPost]
        public JsonResult Documento(int iddet, HttpPostedFileBase file)
        {
            try
            {
                SASEntities db = new SASEntities();
                var can = db.DetalleCanalizacion.FirstOrDefault(i => i.IdDetalleCanalizar == iddet).IdCanalizarFk;
                var sol = db.Canalizacion.FirstOrDefault(i => i.IdCanalizacion == can).IdSolicitudFk;

                Documentos doc = new Documentos();
                doc.IdDetalleCanalizarFk = iddet;
                if (file != null)
                {
                    int length = file.ContentLength;
                    byte[] buffer = new byte[length];
                    file.InputStream.Read(buffer, 0, length);
                    doc.Documento = buffer;
                }
                doc.Nombre = "Inicio_Solicitud_" + sol + "_" + DateTime.Now.Year.ToString();
                doc.Tipo = file.ContentType;

                doc.CrearDoc();

                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes",
                    msj = "El documento se a guardado correctamente"
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes",
                    msj = "El documento no se a podido guardar, intentalo nuevamente"
                });
            }
        }

        public ActionResult Validaciones(int esc, string sol)
        {
            SASEntities db = new SASEntities();
            Validacion_Requerimientos vr = new Validacion_Requerimientos();

            vr.Solicitudes = db.Solicitudes.FirstOrDefault(i => i.Folio == sol);

            vr.FolioSolicitud = sol;
            Contacto c = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == esc);

            vr.Escuela = db.Escuela.FirstOrDefault(i => i.IdEscuela == esc);
            vr.Escuela.Celular = c.Celular;
            vr.Escuela.NombreDirector = c.Nombre;
            vr.Escuela.Telefono = c.Telefono;
            vr.Escuela.EmailDirector = c.Email;

            vr.validacion = Validacion.Crear(esc);
            vr.requerimientos = Requerimientos.Crear(esc);

            Validacion v = db.Validacion.FirstOrDefault(i => i.IdEscuelaFk == esc);

            vr.Aulas = EspacioEducativoDet.ContarAulas(v == null ? 0 : v.IdValidar);
            vr.Laboratorios = EspacioEducativoDet.ContarLaboratorios(v == null ? 0 : v.IdValidar);
            vr.Talleres = EspacioEducativoDet.ContarTalleres(v == null ? 0 : v.IdValidar);
            vr.Anexos = EspacioEducativoDet.ContarAnexos(v == null ? 0 : v.IdValidar);

            if (v != null)
            {
                vr.Matricula = db.Matricula.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                             db.Matricula.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new Matricula();

                vr.Entorno = db.Entorno.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                             db.Entorno.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new Entorno();

                if (vr.Entorno != null)
                {
                    vr.Entorno.Rio_Arrollo = false;
                    vr.Entorno.AmenazaVial = false;
                    vr.Entorno.Comercio = false;
                    vr.Entorno.DerechoVia = false;
                    vr.Entorno.Gasolinera = false;
                }

                vr.Croquis = db.Croquis.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                             db.Croquis.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new Croquis();

                vr.ServicioMunicipal = db.ServicioMunicipal.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                                       db.ServicioMunicipal.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new ServicioMunicipal();

                vr.AlmacenamientoDren = db.AlmacenamientoDren.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                                        db.AlmacenamientoDren.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new AlmacenamientoDren();

                vr.EnergiaElectrica = db.EnergiaElectrica.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                                      db.EnergiaElectrica.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new EnergiaElectrica();

                vr.EspacioMultiple = db.EspacioMultiple.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                                     db.EspacioMultiple.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new EspacioMultiple();

                vr.EspacioEducativo = db.EspacioEducativo.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) != null ?
                                      db.EspacioEducativo.FirstOrDefault(i => i.IdValidarFk == v.IdValidar) : new EspacioEducativo();

                vr.EspacioEducativoDet = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk ==
                                         db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == v.IdValidar).IdEspacioEducativo).ToArray() != null ? db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk ==
                                         db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == v.IdValidar).IdEspacioEducativo).ToArray() :
                                         null;
            }

            Requerimientos r = db.Requerimientos.FirstOrDefault(i => i.IdEsceulaFk == esc);
            if(r != null)
            {
                vr.ComponenteI = db.ComponenteI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                 db.ComponenteI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteI();
                vr.ComponenteII = db.ComponenteII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                  db.ComponenteII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteII();
                vr.ComponenteIII = db.ComponenteIII.Where(i => i.IdRequerimientoFk == r.IdRequerimiento).ToArray() != null ?
                                   db.ComponenteIII.Where(i => i.IdRequerimientoFk == r.IdRequerimiento).ToArray() : null;
                vr.ComponenteVI = db.ComponenteVI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                  db.ComponenteVI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteVI();
                vr.ComponenteV = db.ComponenteV.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                 db.ComponenteV.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteV();
                vr.ComponenteVI = db.ComponenteVI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                  db.ComponenteVI.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteVI();
                vr.ComponenteVII = db.ComponenteVII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                   db.ComponenteVII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteVII();
                vr.ComponenteVIII = db.ComponenteVIII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) == null ?
                                    db.ComponenteVIII.FirstOrDefault(i => i.IdRequerimientoFk == r.IdRequerimiento) : new ComponenteVIII();
            }

            return View(vr);
        }

        [HttpPost]
        public JsonResult Validaciones(Validacion_Requerimientos vr) {
            return Json(false);
        }

        public FileContentResult GetCroquis(int id)
        {
            SASEntities db = new SASEntities();
            var croquis = db.Croquis.FirstOrDefault(i => i.IdCroquis == id);
            if (croquis != null || croquis.DocCroquis.Count() > 0)
            {

                string type = string.Empty;
                type = croquis.Tipo;
                var file = File(croquis.DocCroquis, type);
                return file;
            }
            else
            {
                return null;
            }
        }
    }
}