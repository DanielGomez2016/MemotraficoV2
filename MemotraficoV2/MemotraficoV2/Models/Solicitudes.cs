using System;
using System.Collections.Generic;
using System.Linq;
using MemotraficoV2.Models;
using MemotraficoV2.Models.Colecciones;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MemotraficoV2.ViewModels;

namespace MemotraficoV2.Models
{

    [MetadataType(typeof(mSolicitudes))]
    public partial class Solicitudes
    {
        public class mSolicitudes
        {
            [Required]
            [Display(Name = "Tipo Asunto")]
            public string IdTipoAsuntoFk { get; set; }

            [Display(Name = "Tipo Procedencia")]
            public string IdTipoProcedenciaFk { get; set; }

            [Display(Name = "Procedencia")]
            public string IdProcedenciaFk { get; set; }

            [Display(Name = "Plantel Educativo")]
            public string IdEscuelaFk { get; set; }

            [Display(Name = "Fecha Entrega")]
            public string FechaEntrega { get; set; }

            [Display(Name = "Beneficiario")]
            public string IdBeneficiarioFk { get; set; }

            [Display(Name = "Estatus")]
            public string IdEstatusFk { get; set; }
        }

        public string UltimoRol { get; set; }
        public string uactual { get; set; }
        public string Comentario { get; set;}
        public Boolean validacion { get; set; }
        [Required]
        public string telEscuela { get; set; }
        [Required]
        public string emailEscuela { get; set; }
        [Required]
        public string directorPlantel { get; set; }


        public int Crear()
        {
            SASEntities db = new SASEntities();
            Solicitudes s = new Solicitudes();
            s.Asunto = Asunto;
            if (IdEscuelaFk > 0)
            {
                s.IdEscuelaFk = IdEscuelaFk;
            }
            if (IdBeneficiarioFk > 0)
            {
                s.IdBeneficiarioFk = IdBeneficiarioFk;
            }
            s.IdEstatusFk = ListaEstatus.INICIADO;
            s.IdProcedenciaFk = IdProcedenciaFk;
            s.IdNivelImportanciaFk = ListaImportancia.NORMAL;//nivel de importancia normal
            s.IdTipoProcedenciaFk = IdTipoProcedenciaFk;
            s.Folio = GenerarFolio(FechaEntrega);
            s.FechaEntrega = FechaEntrega;
            db.Solicitudes.AddObject(s);
            db.SaveChanges();
            return s.IdSolicitud;
        }

        public int CrearImporte()
        {
            SASEntities db = new SASEntities();

            if (IdEscuelaFk > 0)
            {
                IdEscuelaFk = IdEscuelaFk;
            }
            if (IdBeneficiarioFk > 0)
            {
                IdBeneficiarioFk = IdBeneficiarioFk;
            }
            IdEstatusFk = ListaEstatus.INICIADO;
            IdNivelImportanciaFk = ListaImportancia.NORMAL;//nivel de importancia normal
            db.Solicitudes.AddObject(this);
            db.SaveChanges();
            return IdSolicitud;
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            Solicitudes s = db.Solicitudes.FirstOrDefault(j => j.IdSolicitud == IdSolicitud);
            s.Programa = Programa;
            s.FechaValidacion = FechaValidacion;

            db.SaveChanges();

        }

        public void EditarEstatus()
        {
            SASEntities db = new SASEntities();
            Solicitudes s = db.Solicitudes.FirstOrDefault(j => j.IdSolicitud == IdSolicitud);
            s.IdEstatusFk = IdEstatusFk;

            db.SaveChanges();

        }

        public string GenerarFolio(DateTime? f)
        {
            SASEntities db = new SASEntities();
            var contador = db.Solicitudes.Where(i => i.FechaEntrega.Value.Year == f.Value.Year).Count();
            contador = contador + 1;
            var cadena = "S-" + contador.ToString("0000") + "-" + f.Value.Year;
            return cadena;
        }

        public static dynamic AutocompleteEsc(string term)
        {
            SASEntities db = new SASEntities();
            term = term.Trim();

            IQueryable<Escuela> query = db.Escuela;

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(i => i.Clave.Contains(term) || i.Nombre.Contains(term));
            }

            return query.OrderBy(i => i.Clave)
                        .Select(i => new
                        {
                            value = i.IdEscuela,
                            name = i.Clave +" "+i.Nombre
                        })
                        .Take(15)
                        .ToArray();
        }

        public static dynamic AutocompleteBen(string term)
        {
            SASEntities db = new SASEntities();
            term = term.Trim();

            IQueryable<Beneficiario> query = db.Beneficiario;

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(i => i.Nombre.Contains(term));
            }

            return query.OrderBy(i => i.Nombre)
                        .Select(i => new
                        {
                            value = i.IdBeneficiario,
                            name = i.Nombre
                        })
                        .Take(15)
                        .ToArray();
        }

        public static AnexGRIDResponde AllSolicitudes(AnexGRID grid)
        {
            grid.Inicializar();
            
            try
            {
                SASEntities db = new SASEntities();
                var s = db.Solicitudes.AsQueryable();

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
                   
                }
                else
                {
                    s = db.Solicitudes.AsQueryable();
                    //el administrador de solicitudes solo puede ver aquellas solicitudes que han sido registradas, ademas de las canceladas por administrador de dependencia
                    if (roles == ListaRoles.ADMINISTRADOR_SOLICITUDES)
                    {

                        s = s.OrderByDescending(m => m.Folio)
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
                        s = s.OrderByDescending(m => m.Folio)
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

                        s = s.OrderByDescending(m => m.Folio)
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

                if (grid.columna == "Name")
                {
                    s = grid.columna_orden == "DESC" ? s.OrderBy(x => x.IdSolicitud) : s.OrderByDescending(x => x.IdSolicitud);
                }

                foreach (var f in grid.filtros)
                {
                    if (f.columna == "Name")
                        s = s.Where(x => x.Escuela.Nombre.Contains(f.valor) || x.Beneficiario.Nombre.Contains(f.valor));
                    if (f.columna == "folio")
                        s = s.Where(x => x.Folio.Contains(f.valor));
                    if (f.columna == "fecha")
                        s = s.Where(x => x.FechaEntrega.ToString().Contains(f.valor));
                    if (f.columna == "estatus")
                        s = s.Where(x => x.Estatus.Estatus1.Contains(f.valor));
                    if (f.columna == "nivel")
                        s = s.Where(x => x.NivelImportancia.Nivel.Contains(f.valor));
                }

                var query = s.Select(x => new SolicitudGrid
                {
                    Id = x.IdSolicitud,
                    Name = x.IdEscuelaFk > 0 || x.IdEscuelaFk != null ? x.Escuela.Nombre : x.Beneficiario.Nombre,
                    folio = x.Folio,
                    fecha = x.FechaEntrega.ToString().Substring(0, 11),
                    estatus = x.IdEstatusFk,
                    nivel = x.IdNivelImportanciaFk,
                    colornivel = x.NivelImportancia.Color
                }).ToList();

                var data = query
                           .Skip(grid.pagina)
                           .Take(grid.limite)
                           .ToList();

                var total = query.Count();

                grid.SetData(data, total);
            }
            catch (Exception e)
            {

            }

            return grid.responde();
        }

    }
}