using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial  class Croquis
    {
        public void Crear() {
            SASEntities db = new SASEntities();
            db.Croquis.AddObject(this);
            db.SaveChanges();
        }
        public void Editar() {
            SASEntities db = new SASEntities();
            Croquis c = db.Croquis.FirstOrDefault(i => i.IdCroquis == IdCroquis);
            c.TipoSuelo = TipoSuelo;
            c.Predio = Predio;
            db.SaveChanges();
        }

        public void EditarDocumento()
        {
            SASEntities db = new SASEntities();
            Croquis c = db.Croquis.FirstOrDefault(i => i.IdCroquis == IdCroquis);
            c.Tipo = Tipo;
            c.DocCroquis = DocCroquis;
            db.SaveChanges();
        }

        public static Croquis ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            Croquis c = new Croquis();
            var registro = db.Croquis.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c.IdValidarFk = valor;
                c.Crear();
            }
            c = db.Croquis.FirstOrDefault(i => i.IdValidarFk == valor);
            return c;
        }
    }
}