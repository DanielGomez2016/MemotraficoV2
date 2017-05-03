using System;
using System.Collections.Generic;
using System.Linq;
using MemotraficoV2.Models;
using MemotraficoV2.Models.Colecciones;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Solicitudes
    {
        public string UltimoRol { get; set; }
        public string uactual { get; set; }
        public string Comentario { get; set;}

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
            s.IdTipoProcedenciaFk = IdTipoProcedenciaFk;
            s.Folio = GenerarFolio(FechaEntrega);
            s.FechaEntrega = FechaEntrega;
            db.Solicitudes.AddObject(s);
            db.SaveChanges();
            return s.IdSolicitud;
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