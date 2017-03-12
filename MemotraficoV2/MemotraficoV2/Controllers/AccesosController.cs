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
    [Authorize, Acceso]
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
                var m = x.AccesoSistemaRol.FirstOrDefault(j => j.IdInstituto == institucion && j.IdRol == idRol) != null;
                if (m)
                {
                    tipo = "success";
                    icono = "check-square-o";
                    mensaje = "dasactivado";
                }
                else
                {
                    tipo = "danger";
                    icono = "square-o";
                    mensaje = "Activado";
                }

                    string row = "<tr><td>"+x.controlador+
                    "</td><td>"+x.accion+
                    "</td><td>"+x.descripcion+
                    "</td><td><button class=\"btn btn-"+tipo+ " pull-right\" data-rol=\""+idRol+"\" data-id=" + x.IdAccesoSistema+" data-msg="+mensaje+" data-activar=\"\"><i class=\"fa fa-"+icono+"\"></i> "+mensaje+"</button></td></tr>";

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
        public ActionResult SistemaBC(string controlador, string descripcion)
        {
            List<AccesoSistema> list = AccesoSistema.Buscar(controlador, descripcion, null);

            return PartialView("_SistemaBC",list);
        }

        #endregion Edicion Acceso Controladores

        [Ignore]
        public ActionResult CargarMenu(string[] roles, int? cliente)
        {
            return View(AccesoSistema.listarPorRoles(Roles.GetRolesForUser(), cliente));
        }

        [Ignore]
        public ActionResult SinAcceso()
        {
            return View();
        }
    }
}