using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Requerimientos
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.Requerimientos.AddObject(this);
            db.SaveChanges();
        }

        public static Requerimientos ObtenerRegistro(int escuela)
        {
            SASEntities db = new SASEntities();
            Requerimientos r = new Requerimientos();
            var registro = db.Requerimientos.Where(i => i.IdEsceulaFk == escuela).Count() > 0 ? false : true;
            if (registro)
            {
                r.IdEsceulaFk = escuela;
                r.Crear();
            }
            r = db.Requerimientos.FirstOrDefault(i => i.IdEsceulaFk == escuela);
            return r;
        }

        public int CountRegistro(int esc)
        {
            SASEntities db = new SASEntities();
            var valor = db.Requerimientos.Where(i => i.IdEsceulaFk == esc).Count();
            return valor;
        }
    }
}