using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class ComponenteIII
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteIII.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteIII c3 = db.ComponenteIII.FirstOrDefault(i => i.IdRequerimientoFk == IdRequerimientoFk);
            c3.Concepto = Concepto;
            c3.Solicita = Solicita;
            c3.Requiere = Requiere;
            db.SaveChanges();
        }

        public static ComponenteIII[] ObtenerRegistros(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteIII[] c3 = null;
            var registro = db.ComponenteIII.Where(i => i.IdRequerimientoFk == valor).Count() > 0 ? true : false;
            if (registro)
            {
                c3 = db.ComponenteIII.Where(i => i.IdRequerimientoFk == valor).ToArray();
            }

            return c3;
        }

        public static void EliminaRegistros(int valor) {
            SASEntities db = new SASEntities();
            List<ComponenteIII> lc3 = db.ComponenteIII.Where(i => i.IdRequerimientoFk == valor).ToList();

            foreach(var x in lc3)
            {
                db.DeleteObject(x);
                db.SaveChanges();
            }
        }
    }
}