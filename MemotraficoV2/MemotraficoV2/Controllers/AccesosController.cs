using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.Filters;
using System.Web.Security;
using IdentitySample.Controllers;
using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using MemotraficoV2.Models.Colecciones;

namespace MemotraficoV2.Controllers
{
    [Authorize]
    public class AccesosController : Controller
    {
        #region rolemanager y usermanager
        public AccesosController()
        {
        }

        public AccesosController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        #endregion

        #region Dar acceso a los roles

        public ActionResult Index()
        {
            var x = RoleManager.Roles
                .Where(i => i.Name != Models.Colecciones.ListaRoles.ADMINISTRATOR)
                .OrderBy(i => i.Name)
                .Select(i => new { id=i.Id, name = i.Name})
                .ToArray();
            List<AccesoSistema.roles> roles = new List<AccesoSistema.roles>();

            foreach(var j in x)
            {
                var rol = new AccesoSistema.roles();
                rol.id = j.id;
                rol.name = j.name;
                roles.Add(rol);
            }

            return View(roles);
        }

        [Ignore]
        public ActionResult Accesos(string rol)
        {
            ApplicationRole sr = RoleManager.Roles.FirstOrDefault(i => i.Id == rol);
            if (sr == null)
                sr = new ApplicationRole();

            ViewBag.Controladores = AccesoSistema.listar().Select(i => i.controlador).Distinct();
            ViewBag.Rolname = sr.Name;
            ViewBag.Rolid = sr.Id;
            var x = Institucion.getnameInstitucion();
            ViewBag.Instituciones = x;
            return View("_Accesos");
        }

        public JsonResult Buscar(string idRol, string controlador, string descripcion)
        {
            SASEntities db = new SASEntities();
            int? institucion = Usuarios.GetInstitucion();
            IQueryable<AccesoSistema> query = db.AccesoSistema.Where(i => i.activo == true);

            if (!string.IsNullOrEmpty(controlador))
                query = query.Where(i => i.controlador == controlador);

            if (!string.IsNullOrEmpty(descripcion))
                query = query.Where(i => i.descripcion.Contains(descripcion));

            List<string> rows = new List<string>();

            foreach(var x in query)
            {
                var tipo = "";
                var icono = "";
                var mensaje = "";
                var btnmsj = "";
                var m = x.AccesoSistemaRol.FirstOrDefault(j => j.IdInstituto == institucion && j.IdRol == idRol) != null;
                if (m)
                {
                    tipo = "success";
                    icono = "check-square-o";
                    mensaje = "desactivado";
                    btnmsj = "Permiso Activo";
                }
                else
                {
                    tipo = "danger";
                    icono = "square-o";
                    mensaje = "activado";
                    btnmsj = "Sin Permiso";
                }

                    string row = "<tr><td>"+x.controlador+
                    "</td><td>"+x.accion+
                    "</td><td>"+x.descripcion+
                    "</td><td><button class=\"btn btn-"+tipo+ " pull-right\" data-rol=\""+idRol+"\" data-id=" + x.IdAccesoSistema+" data-msg="+mensaje+" data-activar=\"\"><i class=\"fa fa-"+icono+"\"></i> "+ btnmsj + "</button></td></tr>";

                rows.Add(row);
            }

            return Json(new
            {
                total = query.Count(),
                datos = rows.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivarAcceso(int idaccesoSistema, string idRol, int?[] instituciones)
        {
            try
            {
                if (instituciones != null)
                {
                    foreach (int i in instituciones)
                        AccesoSistema.activar(idaccesoSistema, idRol, i);
                }
                else
                    AccesoSistema.activar(idaccesoSistema, idRol);

                return Json(new
                {
                    result = true
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    message = e.InnerException != null ? e.InnerException.Message : e.Message
                });
            }
        }

        [HttpPost, Ignore]
        public JsonResult AgregarTodos(string rol, string controlador, string descripcion)
        {
            try
            {
                using (SASEntities db = new SASEntities())
                {
                    int institucion = Usuarios.GetInstitucion();
                    IQueryable<AccesoSistema> query = db.AccesoSistema.Where(i =>  i.activo == true);
                    IQueryable<AccesoSistemaRol> qasr = db.AccesoSistemaRol.Where(i => i.IdInstituto == institucion && i.IdRol == rol);


                    if (!string.IsNullOrEmpty(controlador))
                        query = query.Where(i => i.controlador == controlador);

                    if (!string.IsNullOrEmpty(descripcion))
                        query = query.Where(i => i.descripcion.Contains(descripcion));

                    foreach (var r in query.ToList())
                    {
                        if (!qasr.Any(i => i.IdAccesoSistema == r.IdAccesoSistema))
                        {
                            AccesoSistemaRol ar = new AccesoSistemaRol();

                            ar.IdInstituto = institucion;
                            ar.IdRol = rol;
                            ar.IdAccesoSistema = r.IdAccesoSistema;

                            db.AccesoSistemaRol.AddObject(ar);
                        }

                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        result = true
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    message = e.InnerException != null ? e.InnerException.Message : e.Message
                });
            }
        }

        [HttpPost, Ignore]
        public JsonResult QuitarTodos(string rol, string controlador, string descripcion)
        {
            try
            {
                using (SASEntities db = new SASEntities())
                {
                    int institucion = Usuarios.GetInstitucion();
                    IQueryable<AccesoSistema> query = db.AccesoSistema.Where(i => i.activo == true);
                    IQueryable<AccesoSistemaRol> qasr = db.AccesoSistemaRol.Where(i => i.IdInstituto == institucion && i.IdRol == rol);


                    if (!string.IsNullOrEmpty(controlador))
                        query = query.Where(i => i.controlador == controlador);

                    if (!string.IsNullOrEmpty(descripcion))
                        query = query.Where(i => i.descripcion.Contains(descripcion));

                    foreach (var r in query.ToList())
                    {

                        AccesoSistemaRol ar = qasr.FirstOrDefault(i => i.IdAccesoSistema == r.IdAccesoSistema);
                        if (ar != null)
                        {
                            db.AccesoSistemaRol.DeleteObject(ar);
                        }
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        result = true
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    message = e.InnerException != null ? e.InnerException.Message : e.Message
                });
            }
        }

        #endregion

        #region Edicion Acceso Controladores
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        public ActionResult Sistema()
        {
            List<AccesoSistema> list = AccesoSistema.Buscar("", "", null);
            ViewBag.Controladores = new SelectList(AccesoSistema.GetAllControladores());
            return View();
        }

        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        [HttpPost]
        public JsonResult Create(AccesoSistema acceso)
        {
            try
            {
                SASEntities db = new SASEntities();
                db.AccesoSistema.AddObject(acceso);
                db.SaveChanges();
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        /// <summary>
        /// Edita la informacion de un 
        /// AccesoSistema(Controller, Accion, Descripcion)
        /// </summary>
        /// <param name="id">idAccesoSistema</param>
        /// <returns></returns>
        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        public ActionResult Editar(int id)
        {
            SASEntities db = new SASEntities();
            return View(db.AccesoSistema.First(i => i.IdAccesoSistema == id));
        }

        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        [HttpPost]
        public JsonResult Editar(int idAccesoSistema, string descripcion)
        {
            return Json(AccesoSistema.Editar(idAccesoSistema, descripcion));
        }

        /// <summary>
        /// Activa un controlador:
        ///     true: se completo correctamente
        /// </summary>
        /// <param name="id">idAccesoSistema</param>
        /// <returns></returns>
        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        public JsonResult SistemaActivarControlador(int id)
        {
            bool response = false;
            try
            {
                response = AccesoSistema.SistemaActivarControlador(id);
            }
            catch { }
            return Json(response);
        }

        /// <summary>
        /// Desactiva un controlador:
        ///     true: se completo correctamente
        /// </summary>
        /// <param name="id">idAccesoSistema</param>
        /// <returns></returns>
        [Authorize(Roles = ListaRoles.ADMINISTRATOR), Ignore]
        public JsonResult SistemaDesactivarControlador(int id)
        {
            bool response = false;
            try
            {
                response = AccesoSistema.SistemaDesactivarControlador(id);
            }
            catch { }
            return Json(response);
        }

        /// <summary>
        /// Diferencias de Buscar(...):
        ///     - No toma en cuenta el cliente actual
        ///     - Busca en todos los controladores y acciones sin importar el campo activo
        ///     - Esta accesible vía POST 
        ///     - Solo para SuperUsuarios
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = ListaRoles.ADMINISTRATOR)]
        public ActionResult SistemaBuscarControlador(string controlador, string descripcion)
        {
            List<AccesoSistema> list = AccesoSistema.Buscar(controlador, descripcion, null);

            List<string> rows = new List<string>();

            foreach (var x in list)
            {
                var title = x.activo == true ? "Desactivar" : "Activar";
                var action = x.activo == true ? "SistemaDesactivarControlador" : "SistemaActivarControlador";
                var clase = x.activo == true ? "btn btn-sm btn-danger" : "btn btn-sm btn-primary";
                var ruta = x.controlador + "/" + x.accion;
                var btn1 = "<button class=\"" + clase + "\" "+
                           "data-title=\"" + title + "\" "+
                           "data-url=\"/Accesos/\" " +
                           "data-action=\"" + action + "\">" + title + "</button>";

                var btn2 = "<button class=\"btn btn-sm btn-info\" data-editar=\"editar\" data-action data-url=\"/Accesos/Editar\">Editar</button>";

                string row = "<tr><td>"+x.controlador+"</td>" +
                             "<td>"+x.accion+"</td>" +
                             "<td>"+x.descripcion+"</td>" +
                             "<td class=\"pull-right\">"
                                + "<div class=\"btn-group\" data-route=\""+ruta+"\" data-id=\""+x.IdAccesoSistema+"\">" + btn1 + btn2 +"</div></td></tr>";

                rows.Add(row);
            }

            return Json(new
            {
                total = rows.Count(),
                datos = rows.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Edicion Acceso Controladores

        [Ignore]
        public ActionResult CargarMenu()
        {
            return View(AccesoSistema.listarPorRoles(Usuarios.Roles(), Usuarios.GetInstitucion()));
        }

        [Ignore]
        public ActionResult SinAcceso()
        {
            return View();
        }
    }
}