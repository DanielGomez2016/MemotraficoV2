using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class DetalleCanalizacion
    {
        public int Crear()
        {
            SASEntities db = new SASEntities();
            db.DetalleCanalizacion.AddObject(this);
            db.SaveChanges();

            return IdDetalleCanalizar;
        }
    }
}