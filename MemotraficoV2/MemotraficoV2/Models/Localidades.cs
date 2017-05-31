using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Localidades
    {
        public void Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Localidades c = db.Localidades.FirstOrDefault(i => i.IdLocalidad == this.IdLocalidad);
                c.Nombre = Nombre;
                c.IdMunicipioFk = IdMunicipioFk;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int IdLocalidades(string municipio)
        {
            SASEntities db = new SASEntities();
            return db.Localidades.FirstOrDefault(i => i.Nombre.Contains(municipio)).IdLocalidad;
        }
    }
}