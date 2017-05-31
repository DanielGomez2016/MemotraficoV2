using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Municipios
    {
        public void Editar()
        {
            SASEntities db = new SASEntities();
            Municipios m = db.Municipios.FirstOrDefault(i => i.IdMunicipio == IdMunicipio);
            m.Nombre = Nombre;

            db.SaveChanges();
        }

        public static int IdMunicipios(string municipio)
        {
            SASEntities db = new SASEntities();
            return db.Municipios.FirstOrDefault(i => i.Nombre.Contains(municipio)).IdMunicipio;
        }
    }
}