using MemotraficoV2.Models.Colecciones;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace MemotraficoV2.Models
{
    public partial class AccesoSistema
    {
        public class roles
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        internal static bool Editar(int id, string descripcion)
        {
            SASEntities db = new SASEntities();
            AccesoSistema acceso = db.AccesoSistema.FirstOrDefault(i => i.IdAccesoSistema == id);
            acceso.descripcion = descripcion;
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Valida si el usuario actual tiene acceso a la URL solicitada
        /// </summary>
        /// <param name="controller">Controlador</param>
        /// <param name="action">Acción</param>
        /// <param name="roles">Roles a validar</param>
        /// <returns></returns>
        public static bool tieneAcceso(string controller, string action, string rol)
        {
            SASEntities db = new SASEntities();
                //devuelve la institucion que esta en session
                int institucion = Usuarios.GetInstitucion();

                //si no tiene roles asignados el usuario no tiene acceso a el sistema
                if (rol == null)
                    return false;

                //admin tiene acceso total
                if (rol.Contains("Admin")) return true;

                AccesoSistema perm = db.AccesoSistema.FirstOrDefault(i => i.activo == true && i.controlador == controller && i.accion == action);

                //si perm 
                if (perm == null)
                    return true;
                else
                    return perm.AccesoSistemaRol.Any(i => rol.Contains(i.AspNetRoles.Name) && i.IdInstituto == institucion);
            
        }

        public static void activar(int idAccesoSistema, string idRol, int? instituciones = null)
        {
            SASEntities db = new SASEntities();
            if (instituciones == null)
                instituciones = Usuarios.GetInstitucion();

            AccesoSistemaRol acceso = db.AccesoSistemaRol.FirstOrDefault(i => i.IdAccesoSistema == idAccesoSistema && i.IdRol == idRol && i.IdInstituto == instituciones);

            if (acceso != null)
                db.DeleteObject(acceso);
            else
            {
                AccesoSistemaRol asr = new AccesoSistemaRol();
                asr.IdAccesoSistema = idAccesoSistema;
                asr.IdRol = idRol;
                asr.IdInstituto = instituciones.Value;
                db.AccesoSistemaRol.AddObject(asr);
            }
            db.SaveChanges();
        }

        public static List<AccesoSistema> Buscar(string controlador, string descripcion, bool? activo = true)
        {
            SASEntities db = new SASEntities();
            IQueryable<AccesoSistema> query = db.AccesoSistema.Where(i => activo.HasValue ? i.activo == activo : true);

            if (!string.IsNullOrEmpty(controlador))
                query = query.Where(i => i.controlador == controlador);

            if (!string.IsNullOrEmpty(descripcion))
                query = query.Where(i => i.descripcion.Contains(descripcion));

            return query
                .OrderBy(i => new { i.controlador, i.accion })
                .ToList();
        }

        public static List<string> GetAllControladores()
        {
            List<AccesoSistema> list = Buscar("", "", null);
            return list.Select(l => l.controlador).Distinct().ToList();
        }

        public static List<AccesoSistema> listar()
        {
            SASEntities db = new SASEntities();
            return db.AccesoSistema.OrderBy(i => new { i.controlador, i.accion }).ToList();
        }

        public static List<AccesoSistema> listarPorRoles(string roles, int? institucion = null)
        {
            SASEntities db = new SASEntities();
            if (institucion == null)
                institucion = Usuarios.GetInstitucion();

            var query = db.AccesoSistemaRol
                          .Where(x => 
                                 x.IdInstituto == institucion &&
                                 x.AspNetRoles.Name == roles).Select(x => x.AccesoSistema)
                          .ToList();

            return query;
        }

        public static bool SistemaCambiarEstatusControlador(int idAccesoSistema, bool nuevoEstatus)
        {
            SASEntities db = new SASEntities();
            AccesoSistema acceso = db.AccesoSistema.FirstOrDefault(i => i.IdAccesoSistema == idAccesoSistema);
            acceso.activo = nuevoEstatus;
            db.SaveChanges();
            return true;
        }

        public static bool SistemaActivarControlador(int idAccesoSistema)
        {
            return SistemaCambiarEstatusControlador(idAccesoSistema, true);
        }

        public static bool SistemaDesactivarControlador(int idAccesoSistema)
        {
            return SistemaCambiarEstatusControlador(idAccesoSistema, false);
        }
    }
}