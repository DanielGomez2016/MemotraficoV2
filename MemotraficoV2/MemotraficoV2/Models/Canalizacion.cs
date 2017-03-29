using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Canalizacion
    {
        public int Crear()
        {
            SASEntities db = new SASEntities();
            db.Canalizacion.AddObject(this);
            db.SaveChanges();

            return IdCanalizacion;
        }
    }
}