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
    }
}