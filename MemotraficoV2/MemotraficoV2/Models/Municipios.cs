using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Municipios
    {

        public int Crear()
        {
            SASEntities db = new SASEntities();
            db.Municipios.AddObject(this);
            db.SaveChanges();

            return IdMunicipio;
        }
        public int Editar()
        {
            SASEntities db = new SASEntities();
            Municipios m = db.Municipios.FirstOrDefault(i => i.IdMunicipio == IdMunicipio);
            m.Nombre = Nombre;
            db.SaveChanges();

            return IdMunicipio;
        }

        public int get(string n)
        {
            SASEntities db = new SASEntities();
            Municipios m = db.Municipios.FirstOrDefault(i => i.Nombre == n);

            return m.IdMunicipio;
        }

        public static int IdMunicipios(string municipio)
        {
            SASEntities db = new SASEntities();
            return db.Municipios.FirstOrDefault(i => i.Nombre == municipio).IdMunicipio;
        }
    }
}