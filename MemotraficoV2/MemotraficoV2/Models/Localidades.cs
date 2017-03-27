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
    }
}