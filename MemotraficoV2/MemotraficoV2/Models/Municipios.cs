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
            try
            {
                SASEntities db = new SASEntities();
                Municipios m = db.Municipios.FirstOrDefault(i => i.IdMunicipio == this.IdMunicipio);
                m.Nombre = Nombre;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}