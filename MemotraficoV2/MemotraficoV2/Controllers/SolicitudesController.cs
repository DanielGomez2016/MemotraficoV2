using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.ViewModels;
using MemotraficoV2.Models.Colecciones;
using System.Collections;
using MemotraficoV2.Filters;

namespace MemotraficoV2.Controllers
{
    [Authorize, Acceso]
    public class SolicitudesController : Controller
    {
        #region Registro solicitudes
        //modulo para generar registro de solicitudes de cualquier institucion donde vaya a ser la recepcion
        public ActionResult Registro()
        {
            SASEntities db = new SASEntities();
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
                var canalizacion = Canalizacion.Canalizar(v, 0, 0, "", "", ListaEstatus.INICIADO);

                return Json(new
                {
                    result = true,
                    modal = true,
                    idmodal = "model-documento",
                    val = canalizacion,
                    msj = "La solicitud se ha registrado Correctamente"
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
                    dir = "/Solicitudes/Registro",
                    msj = "El documento se ha guardado correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes",
                    msj = "El documento no se ha podido guardar, intentalo nuevamente"
                });
            }
        }

        #endregion

        #region Solicitudes (listado, detalle solicitud, historial solicitud, reabrir solicitud)
        // GET: Solicitudes
        public ActionResult Index()
        {
            SASEntities db = new SASEntities();
            IQueryable<Solicitudes> query = db.Solicitudes;

            //obtiene rol que tiene el usuario que esta en session
            var roles = Usuarios.Roles();

            //obtiene el id de usuario que esta en session
            var usuario = Usuarios.GetUsuario();

            //Obtiene el id de la institucion a la que pertenece
            var instituto = Usuarios.GetInstitucion();

            //Obtiene el id del departamento al que pertenece
            var departamento = Usuarios.GetDepto();

            //obtiene el usuario con rol que es uno mas bajo que al que esta en session y pertenece al mismo instituto
            var RolBajo = Usuarios.RolBajo(roles, instituto);

            //Obtiene el usuario con rol que es uno mas alto al que esta en session y pertenece al mismo instituto
            var RolAlto = Usuarios.RolAlto(roles, instituto);

            //Obtiene el usuario con rol que es igual a el al que esta en session y pertenece al mismo instituto, es para cuando se hace canalizacion de operador a poerador
            var RolIgual = Usuarios.RolIgual(roles, instituto);

            //si eres administrador total, te mostrara todas las solicitudes que existen, aun asi si estas estan canalizadas
            if (roles == ListaRoles.ADMINISTRATOR)
            {
                return View(query);
            }
            else
            {
                //el administrador de solicitudes solo puede ver aquellas solicitudes que han sido registradas, ademas de las canceladas por administrador de dependencia
                if(roles == ListaRoles.ADMINISTRADOR_SOLICITUDES) {

                    query = query.OrderByDescending(m => m.Folio)
                                 .Where(i => (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.INICIADO))))
                                 ||
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANCELADO)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolBajo))))
                                 ||
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANALIZADO)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolIgual)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.UsuarioAtiende == null))))
                                 ||
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.ATENDIDA)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolBajo))))
                                 );

                }

                //el administrador de dependencia solo puede ver aquellas solicitudes que han sido canalizadas por el administrador de solicitudes, ademas de las canceladas por operador
                if (roles == ListaRoles.ADMINISTRADOR_DEPENDENCIA)
                {
                    query = query.OrderByDescending(m => m.Folio)
                                 .Where(i => (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANALIZADO))) 
                                 && 
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolAlto))))
                                 ||
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANCELADO)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolBajo))))
                                 ||
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.ATENDIDA)))
                                 &&
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolBajo))))
                                 );
                }

                //el operador solo puede ver aquellas solicitudes que han sido canalizadas por administrador de dependencia y que esten asignadas a el mismo
                if (roles == ListaRoles.OPERADOR)
                {

                    query = query.OrderByDescending(m => m.Folio)
                                 .Where(i => i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANALIZADO)))
                                 &&
                                 (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolAlto)))
                                 ||
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdUsuarioFk == RolIgual))))
                                 ||
                                 i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.UsuarioAtiende == usuario)))
                                 );
                }
            }

            return View(query);
        }

        public ActionResult Detalle(int solicitud, string rol)
        {
            var institucion = Usuarios.GetInstitucion();
            var uactual = Usuarios.Roles();
            SASEntities db = new SASEntities();

            if (rol == ListaRoles.ADMINISTRATOR)
            {
                ViewBag.TipoAsunto = db.TipoAsunto.ToList().Select(i => new { nombre = i.TipoAsunto1, id = i.IdTipoAsunto });
                ViewBag.Institucion = db.Institucion.ToList().Select(i => new { nombre = i.Siglas, id = i.IdInstitucion });
                ViewBag.Departamento = db.Departamento.ToList().Select(i => new { id = i.IdDepartamento, nombre = i.Nombre });
            }

            if (rol == ListaRoles.ADMINISTRADOR_SOLICITUDES || rol == ListaRoles.ADMINISTRADOR_DEPENDENCIA)
            {
                ViewBag.TipoAsunto = db.TipoAsunto.ToList().Select(i => new { nombre = i.TipoAsunto1, id = i.IdTipoAsunto });
                ViewBag.Institucion = db.Institucion.ToList().Select(i => new { nombre = i.Siglas, id = i.IdInstitucion });
                ViewBag.Departamento = db.Departamento.Where(i => i.IdInstitucionFk == institucion).ToList().Select(i => new { id = i.IdDepartamento, nombre = i.Nombre });
            }

            if (rol == ListaRoles.OPERADOR)
            {
                ViewBag.TipoAsunto = db.TipoAsunto.ToList().Select(i => new { nombre = i.TipoAsunto1, id = i.IdTipoAsunto });
                ViewBag.Institucion = db.Institucion.ToList().Select(i => new { nombre = i.Siglas, id = i.IdInstitucion });
                ViewBag.Departamento = db.Departamento.Where(i => i.IdInstitucionFk == institucion).ToList().Select(i => new { id = i.IdDepartamento, nombre = i.Nombre });
            }


            Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == solicitud);
            var usuario = db.DetalleCanalizacion.OrderByDescending(i => i.IdCanalizarFk).OrderByDescending(i => i.FechaCanalizar).FirstOrDefault(i => i.Canalizacion.IdSolicitudFk == s.IdSolicitud).IdUsuarioFk.ToString();
            if(usuario != null)
            s.UltimoRol = Usuarios.Roles(usuario);
            s.uactual = uactual;
            s.validacion = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == s.IdSolicitud).Validacion;
            return View(s);
        }

        public ActionResult Historial(int solicitud, string rol)
        {
            SASEntities db = new SASEntities();
            SolicitudDetalle sd = new SolicitudDetalle();
            sd.Detalles = new List<Detalle>();
            int idcanalizacion = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == solicitud).IdCanalizacion;
            var ix = 0;
            List<DetalleCanalizacion> dc = db.DetalleCanalizacion
                .Where(j => j.IdCanalizarFk == idcanalizacion)
                .OrderByDescending(i => i.IdDetalleCanalizar).ToList();
            ix = dc.Count();
            foreach(var x in dc)
            {
                Detalle d = new Detalle();

                d.FechaCanalizado = x.FechaCanalizar.Value.ToString("dd/MM/yyyy") + " " + x.FechaCanalizar.Value.ToString("HH:mm:ss");
                d.Comentario = x.Comentario;
                d.usuario = Usuarios.GetUsuarioId(x.IdUsuarioFk);
                d.departamento = Departamento.getNombre(x.Departamento);
                d.usuarioatiende = Usuarios.GetUsuarioId(x.UsuarioAtiende);
                d.estatus = x.Estatus.Estatus1;
                d.numregistro = ix;

                switch (x.Estatus.IdEstatus) {
                    case ListaEstatus.INICIADO:
                        d.colorreg = "info";
                        break;
                    case ListaEstatus.CANALIZADO:
                        d.colorreg = "success";
                        break;
                    case ListaEstatus.CANCELADO:
                        d.colorreg = "warning";
                        break;
                    case ListaEstatus.CERRADO:
                        d.colorreg = "danger";
                        break;
                    case ListaEstatus.ATENDIDA:
                        d.colorreg = "success";
                        break;
                    default:
                        d.colorreg = "";
                        break;
                }

                d.docs = db.Documentos.Where(i => i.IdDetalleCanalizarFk == x.IdDetalleCanalizar).ToList();

                sd.Detalles.Add(d);
                ix --;
            }
            sd.solicitud = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == solicitud);

            var ins = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == solicitud).IdInstitucionFk;

            sd.institucion = Institucion.getinstitucionName(ins);
            sd.Detalles.OrderBy(i => i.numregistro);
            return View(sd);
        }

        public ActionResult OpenSolicitud(int solicitud, string rol)
        {
            SASEntities db = new SASEntities();
            int idcanalizacion = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == solicitud).IdCanalizacion;

            Solicitudes s = db.Solicitudes.OrderByDescending(m => m.Folio)
                                 .FirstOrDefault(i => (i.Canalizacion
                                 .Any(j => j.DetalleCanalizacion
                                 .Where(a => a.IdDetalleCanalizar == db.DetalleCanalizacion
                                                                       .OrderByDescending(n => n.IdCanalizarFk)
                                                                       .OrderByDescending(x => x.FechaCanalizar)
                                                                       .Where(n => n.IdCanalizarFk == a.IdCanalizarFk)
                                                                       .Select(l => l.IdDetalleCanalizar)
                                                                       .Max())
                                 .Any(k => (k.IdEstatusFk == ListaEstatus.CANCELADO)))
                                 ));


            DetalleCanalizacion dc = db.DetalleCanalizacion
                                       .OrderByDescending(n => n.IdCanalizarFk)
                                       .OrderByDescending(x => x.FechaCanalizar)
                                       .FirstOrDefault(n => n.IdCanalizarFk == idcanalizacion && n.IdEstatusFk == ListaEstatus.CANCELADO);
            s.Comentario = dc.Comentario;
            return View(s);
        }

        #endregion

        #region canalizaciones
        [HttpPost]
        public JsonResult Canalizar(int TipoAsunto, int Institucion, int IdSolicitud, string Comentario, int Departamento, string Usuario, string rol)
        {
            try
            {
                SASEntities db = new SASEntities();
                Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud);
                int canalizacion = 0;
                switch (rol)
                {
                    case "Administrador de Solicitudes":
                        s.IdTipoAsuntoFk = TipoAsunto;
                        s.IdEstatusFk = ListaEstatus.CANALIZADO;
                        db.SaveChanges();

                        //parametros
                        //solicitud, institucion, departamento, comentario, usuarioasigna, estatus
                        canalizacion = Canalizacion.Canalizar(IdSolicitud, Institucion, Departamento, Comentario, Usuarios.RolBajo(rol, Institucion), ListaEstatus.CANALIZADO);

                        break;
                    case "Administrador de Dependencia":
                        s.IdEstatusFk = ListaEstatus.CANALIZADO;
                        db.SaveChanges();

                        //parametros
                        //solicitud, institucion, departamento, comentario, usuarioasigna, estatus
                        canalizacion = Canalizacion.Canalizar(IdSolicitud,Institucion, Departamento, Comentario, Usuario, ListaEstatus.CANALIZADO);

                        break;
                    case "Operador":
                        s.IdEstatusFk = ListaEstatus.CANALIZADO;
                        db.SaveChanges();

                        //parametros
                        //solicitud, institucion, departamento, comentario, usuarioasigna, estatus
                        canalizacion = Canalizacion.Canalizar(IdSolicitud, Institucion, Departamento, Comentario, Usuario, ListaEstatus.CANALIZADO);

                        break;
                }

                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes/",
                    msj = "La solicitud se a canalizado correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/",
                    msj = "La Solicitud no se a podido canalizar, intente nuevamente"
                });

            }
        }

        [HttpPost]
        public JsonResult Cancelacion(int IdSolicitud, string Comentario)
        {
            try
            {
                SASEntities db = new SASEntities();
                Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud);
                int institucion = Usuarios.GetInstitucion();
                int depto = Usuarios.GetDepto();

                s.IdEstatusFk = ListaEstatus.CANCELADO;
                db.SaveChanges();

                //parametros
                //solicitud, institucion, departamento, comentario, estatus
                Canalizacion.Cancelacion(IdSolicitud, institucion, depto, Comentario, ListaEstatus.CANCELADO);

                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes/",
                    msj = "La solicitud se ha cancelado correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/",
                    msj = "La Solicitud no se ha podido cancelar, intente nuevamente"
                });

            }
        }

        [HttpPost]
        public JsonResult ACSolicitud(int IdSolicitud, string Comentario, string Accion)
        {
            try
            {
                SASEntities db = new SASEntities();
                Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud && i.IdEstatusFk == ListaEstatus.CANCELADO);

                switch (Accion)
                {
                    case "open":
                        s.IdEstatusFk = ListaEstatus.CANALIZADO;
                        db.SaveChanges();

                        //parametros
                        //solicitud, comentario, estatus
                        Canalizacion.canalizarSimple(IdSolicitud, Comentario, ListaEstatus.CANALIZADO);

                        break;
                    case "close":
                        s.IdEstatusFk = ListaEstatus.CANCELADO;
                        db.SaveChanges();

                        //parametros
                        //solicitud, institucion, departamento, comentario, estatus
                        Canalizacion.Cancelacion(IdSolicitud, Usuarios.GetInstitucion(), Usuarios.GetDepto(), Comentario, ListaEstatus.CANCELADO);

                        break;
                }

                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes/",
                    msj = Accion == "open"? "La solicitud se ha abierto correctamente" : "La solicitud se a cancelado correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/",
                    msj = Accion == "open" ? "La Solicitud no se ha podido abrir, intente nuevamente" : "La Solicitud no se a podido cancelar, intente nuevamente"
                });

            }
        }

        [HttpPost]
        public JsonResult CanalizarAvance(int IdSolicitud, string Comentario, HttpPostedFileBase[] files)
        {
            try
            {
                SASEntities db = new SASEntities();
                if (files != null)
                {
                    Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud && i.IdEstatusFk == ListaEstatus.CANCELADO);

                    var dcdoc = Canalizacion.CanalizarAvance(IdSolicitud, Comentario);

                    for (var i = 0; i < files.Count(); i++)
                    {
                        Documentos doc = new Documentos();
                        doc.IdDetalleCanalizarFk = dcdoc;
                        if (files[i] != null)
                        {
                            int length = files[i].ContentLength;
                            byte[] buffer = new byte[length];
                            files[i].InputStream.Read(buffer, 0, length);
                            doc.Documento = buffer;
                        }
                        doc.Nombre = "Documento_Avance_" + (i + 1) + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString();
                        doc.Tipo = files[i].ContentType;

                        doc.CrearDoc();
                    }
                    return Json(new
                    {
                        result = true,
                        dir = "/Solicitudes/",
                        msj = "El avance se guardo correctamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = true,
                        dir = "/Solicitudes/",
                        msj = "Falta documento para confirmar algun avance de la solicitud, intenta nuevamente"
                    });

                }

                
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/",
                    msj = "No se a podido guardar el avance, intente nuevamente"
                });

            }
        }

        [HttpPost]
        public JsonResult CanalizarAtendida(int IdSolicitud, string Comentario, HttpPostedFileBase[] files)
        {
            try
            {
                SASEntities db = new SASEntities();
                if (files != null)
                {
                    Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud);
                    s.IdEstatusFk = ListaEstatus.ATENDIDA;
                    db.SaveChanges();

                    var dcdoc = Canalizacion.CanalizarAtendida(IdSolicitud, Comentario);
                
                    for (var i = 0; i < files.Count(); i++)
                    {
                        Documentos doc = new Documentos();
                        doc.IdDetalleCanalizarFk = dcdoc;
                        if (files[i] != null)
                        {
                            int length = files[i].ContentLength;
                            byte[] buffer = new byte[length];
                            files[i].InputStream.Read(buffer, 0, length);
                            doc.Documento = buffer;
                        }
                        doc.Nombre = "Documento_Atendida_" + (i + 1) + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString();
                        doc.Tipo = files[i].ContentType;

                        doc.CrearDoc();
                    }
                    return Json(new
                    {
                        result = true,
                        dir = "/Solicitudes/",
                        msj = "El avance se guardo correctamente",
                        val = 1
                    });
                }else
                {
                    return Json(new
                    {
                        val = 2,
                        result = false,
                        msj = "Falta algun documento para confirmar que la solicitud fue atendida, intenta nuevamente"
                    });
                }

                
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Solicitudes/",
                    msj = "No se a podido guardar el avance, intente nuevamente"
                });

            }
        }

        [HttpPost]
        public JsonResult CananlizaICHIFE(int IdSolicitud)
        {
            try
            {
                SASEntities db = new SASEntities();
                Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud);
                s.IdEstatusFk = ListaEstatus.CANALIZADO;
                db.SaveChanges();

                var dcdoc = Canalizacion.Canalizar(IdSolicitud, 1, 0, ListaComentarios.CanalizadaIchife, Usuarios.RolIchife(), ListaEstatus.CANALIZADO);

                return Json(new
                {
                    result = true,
                    msj = "Se a canalizado directamente al ICHIFE.",
                    dir = "/Solicitudes/"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    msj = "No se a podido canalizar al ICHIFE, intenta nuevamente.",
                    dir = "/Solicitudes/"
                });

            }
        }

        public JsonResult CerrarSolicitud(int IdSolicitud, string Comentario)
        {
            try
            {
                SASEntities db = new SASEntities();
                Solicitudes s = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == IdSolicitud && i.IdEstatusFk == ListaEstatus.ATENDIDA);
                s.IdEstatusFk = ListaEstatus.CERRADO;
                db.SaveChanges();

                var dcdoc = Canalizacion.CerrarSolicitud(IdSolicitud, Comentario);

                return Json(new
                {
                    result = true,
                    msj = "Se a Cerrado correctamente la solicitud.",
                    dir = "/Solicitudes/"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    msj = "No se a podido cerrar la solicitud, intente nuevamente.",
                    dir = ""
                });

            }
        }

        #endregion

        #region Helpers

        public ActionResult GetDownloadFile(int idfile)
        {
            SASEntities db = new SASEntities();
            var f = db.Documentos.FirstOrDefault(i => i.IdDocumento == idfile);
            if (f != null)
            {
                string type = string.Empty;
                string ext = string.Empty;

                switch (f.Tipo)
                {
                    case "application/pdf":
                        type = "application/pdf";
                        ext = ".pdf";
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        type = "aapplication/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        ext = ".docx";
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        ext = ".xlsx";
                        break;
                    case "application/vnd.ms-excel":
                        type = "application/vnd.ms-excel";
                        ext = ".xls";
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        type = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        ext = ".pptx";
                        break;
                    case "application/vnd.ms-powerpoint":
                        type = "application/vnd.ms-powerpoint";
                        ext = ".ppt";
                        break;
                    case "image/jpeg":
                        type = "image/jpeg";
                        ext = ".jpge";
                        break;
                }
                var file = File(f.Documento, type);
                file.FileDownloadName = f.Nombre+ext;
                return file;
            }
            else
            {
                return null;
            }
        }

        public JsonResult Autocomplete(string term)
        {
            return Json(Solicitudes.AutocompleteEsc(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutocompleteBen(string term)
        {
            return Json(Solicitudes.AutocompleteBen(term), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}