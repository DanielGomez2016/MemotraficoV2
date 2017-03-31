using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Procedencia
    {
        public string NombreMunicipio { get; set; }
        public string NombreLocalidad { get; set; }

        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.Procedencia.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            Procedencia p = db.Procedencia.FirstOrDefault(i => i.IdProcedencia == IdProcedencia);
            p.Procedencia1 = Procedencia1;
            p.Domicilio = Domicilio;
            p.Municipio = Municipio;
            p.Localidad = Localidad;
            p.Contacto = Contacto;
            p.TipoProcedencia = TipoProcedencia;

            db.SaveChanges();

        }
    }
}