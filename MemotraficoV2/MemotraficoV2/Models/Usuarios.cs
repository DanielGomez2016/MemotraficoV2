﻿using System;
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
        public static int GetInstitucion()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser institucion = db.Users.First(i => i.Id == user);

            return institucion.IdInstitucion;
        }

        public static int GetDepto()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser depto = db.Users.First(i => i.Id == user);

            return depto.IdDepartamento;
        }

        public static string GetUsuario()
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();

            ApplicationUser usuario = db.Users.First(i => i.Id == user);

            return usuario.Id;
        }

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

        public static string RolIgual(string rol, int inst)
        {
            SASEntities dbb = new SASEntities();
            AspNetUsers us = dbb.AspNetUsers
                                .FirstOrDefault(i => i.AspNetRoles
                                .Any(m => m.Name == rol && i.IdInstitucion == inst));

            return us.Id;
        }

    }
}