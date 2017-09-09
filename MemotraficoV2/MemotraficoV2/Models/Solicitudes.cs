using System;
using System.Collections.Generic;
using System.Linq;
using MemotraficoV2.Models;
using MemotraficoV2.Models.Colecciones;
using System.Web;
using System.ComponentModel.DataAnnotations;

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

        public void Editar()
        {
            SASEntities db = new SASEntities();
            Solicitudes s = db.Solicitudes.FirstOrDefault(j => j.IdSolicitud == IdSolicitud);
            s.Programa = Programa;
            s.FechaValidacion = FechaValidacion;

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

        
    }
}