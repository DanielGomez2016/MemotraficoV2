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
        public int Crear()
        {
            SASEntities db = new SASEntities();
            Solicitudes s = new Solicitudes();
            s.Asunto = Asunto;
            s.IdEscuelaFk = IdEscuelaFk;
            s.IdEstatusFk = ListaEstatus.INICIADO;
            s.IdProcedenciaFk = IdProcedenciaFk;
            s.IdTipoAsuntoFk = IdTipoAsuntoFk;
            s.IdTipoProcedenciaFk = IdTipoProcedenciaFk;
            s.Folio = GenerarFolio(FechaEntrega);
            s.FechaEntrega = FechaEntrega;
            db.Solicitudes.AddObject(this);
            db.SaveChanges();
            return IdSolicitud;
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

            if (string.IsNullOrEmpty(term))
            {
                query = query.Where(i => i.Clave.Contains(term) || i.Nombre.Contains(term));
            }

            return query.OrderBy(i => i.Clave)
                        .Select(i => new
                        {
                            value = i.IdEscuela,
                            Nombre = i.Clave +" "+i.Nombre
                        })
                        .Take(15)
                        .ToArray();
        }
    }
}