using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentitySample.Controllers;
using MemotraficoV2.Models.Colecciones;

namespace MemotraficoV2.Models
{
    public class Usuarios
    {
        //regresa la institucion del usuario que esta en session actualmente
        public static int GetInstitucion()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser institucion = db.Users.First(i => i.Id == user);

            return institucion.IdInstitucion;
        }

        //regresa el departamento del usuario que esta en session actualmente
        public static int GetDepto()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser depto = db.Users.First(i => i.Id == user);

            return depto.IdDepartamento;
        }

        //regresa el id del usuario que esta en session actualmente
        public static string GetUsuario()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser usuario = db.Users.First(i => i.Id == user);

            return usuario.Id;
        }

        //regresa el id del usuario que esta en session actualmente
        public static string GetUsuario(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser usuario = db.Users.First(i => i.Id == id);

            return usuario.Id;
        }

        //regresa el nombre del usuario pedido por su id
        public static string GetUsuarioId(string id)
        {
            try { 
                ApplicationDbContext db = new ApplicationDbContext();
                ApplicationUser usuario = db.Users.First(i => i.Id == id);
                return usuario.Nombre + " " + usuario.ApellidoPaterno;
            }
            catch
            {
                return "";
            }
        }

        //regresa el rol del usuario que esta en session actualmente
        public static string Roles()
        {
            var rols = "";
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser us = db.Users.First(i => i.Id == user);
            var rol = us.Roles.Select(i => i.RoleId);

            SASEntities dbb = new SASEntities();

            foreach(var r in rol)
            {
                rols = dbb.AspNetRoles.FirstOrDefault(j => j.Id == r).Name;
            }
            

            return rols;
        }

        //regresa el rol de algun usuario por su id
        public static string Roles(string user)
        {
            var rols = "";
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser us = db.Users.First(i => i.Id == user);
            var rol = us.Roles.Select(i => i.RoleId);

            SASEntities dbb = new SASEntities();

            foreach (var r in rol)
            {
                rols = dbb.AspNetRoles.FirstOrDefault(j => j.Id == r).Name;
            }


            return rols;
        }

        //regresa el id de usuario de ADS y que pretenesca al ICHIFE
        public static string RolIchife()
        {
            SASEntities dbb = new SASEntities();
            AspNetUsers us = dbb.AspNetUsers
                             .FirstOrDefault(i => i.AspNetRoles
                             .Any(m => m.Name == ListaRoles.ADMINISTRADOR_SOLICITUDES && i.IdInstitucion == dbb.Institucion.FirstOrDefault(j => j.Siglas == "ICHIFE").IdInstitucion));

            return us.Id;
        }

        //regresa el email del usuario por medio del id
        public static string GetEmail(string id)
        {
            SASEntities dbb = new SASEntities();
            AspNetUsers us = dbb.AspNetUsers
                             .FirstOrDefault(i => i.Id == id);
            return us.Email;
        }

        //regresa el email de todos los ADS que existan
        public static List<string> GetEmailAS()
        {
            SASEntities dbb = new SASEntities();
            List<AspNetUsers> us = dbb.AspNetUsers
                             .Where(i => i.AspNetRoles
                             .Any(m => m.Name == ListaRoles.ADMINISTRADOR_SOLICITUDES)).ToList();

            List<string> emails = new List<string>();
            foreach (var i in us)
            {
                emails.Add(i.Email);
            }

            return emails;
        }

        //regresa el id de usuario dependiendo del un rol mas alto de de la institucion que se necesite
        public static string RolBajo(string rol, int inst)
        {
            SASEntities dbb = new SASEntities();
            if (rol == ListaRoles.ADMINISTRADOR_SOLICITUDES)
            {
                AspNetUsers us = dbb.AspNetUsers
                                       .FirstOrDefault(i => i.AspNetRoles
                                       .Any(m => m.Name == ListaRoles.ADMINISTRADOR_DEPENDENCIA && i.IdInstitucion == inst));
                    return us.Id;
                }
            if (rol == ListaRoles.ADMINISTRADOR_DEPENDENCIA)
            {
                AspNetUsers us = dbb.AspNetUsers
                                       .FirstOrDefault(i => i.AspNetRoles
                                       .Any(m => m.Name == ListaRoles.OPERADOR && i.IdInstitucion == inst));
                return us.Id;
            }

            return "";
        }

        //regresa el id de usuario dependiendo del un rol mas bajo de de la institucion que se necesite
        public static string RolAlto(string rol, int inst)
        {
            SASEntities dbb = new SASEntities();
            if (rol == ListaRoles.OPERADOR)
            {
                AspNetUsers us = dbb.AspNetUsers
                                       .FirstOrDefault(i => i.AspNetRoles
                                       .Any(m => m.Name == ListaRoles.ADMINISTRADOR_DEPENDENCIA && i.IdInstitucion == inst));
                return us.Id;
            }
            if (rol == ListaRoles.ADMINISTRADOR_DEPENDENCIA)
            {
                AspNetUsers us = dbb.AspNetUsers
                                       .FirstOrDefault(i => i.AspNetRoles
                                       .Any(m => m.Name == ListaRoles.ADMINISTRADOR_SOLICITUDES));
                return us.Id;
            }
            if (rol == ListaRoles.ADMINISTRADOR_SOLICITUDES)
            {
                AspNetUsers us = dbb.AspNetUsers
                                       .FirstOrDefault(i => i.AspNetRoles
                                       .Any(m => m.Name == ListaRoles.ADMINISTRADOR_SOLICITUDES && i.IdInstitucion == inst));
                return us.Id;
            }

            return "";
        }

        //regresa el id de usuario del mismo rol y de de la institucion que se necesite
        public static string RolIgual(string rol, int inst)
        {
            SASEntities dbb = new SASEntities();
            AspNetUsers us = dbb.AspNetUsers
                                .FirstOrDefault(i => i.AspNetRoles
                                .Any(m => m.Name == rol && i.IdInstitucion == inst));

            return us.Id;
        }


        public static string CorreoDe(string rol, string tipoaccion)
        {
            try
            {
                SASEntities db = new SASEntities();
                var usuario = "";

                if (rol == ListaRoles.OPERADOR)
                {
                    switch (tipoaccion)
                    {
                        case "Cancelacion":
                            usuario = db.AspNetUsers.FirstOrDefault(x => x.Id == RolAlto(rol, GetInstitucion())).Id;
                            break;
                    }
                }
                else if (rol == ListaRoles.ADMINISTRADOR_SOLICITUDES)
                {
                    switch (tipoaccion)
                    {
                        case "Cancelacion":
                            usuario = db.AspNetUsers.FirstOrDefault(x => x.Id == RolAlto(rol, GetInstitucion())).Id;
                            break;
                    }
                }

                return usuario;
            }
            catch (Exception e)
            {
                return "";
            }

        }

    }
}