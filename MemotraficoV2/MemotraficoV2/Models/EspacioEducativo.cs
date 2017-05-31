using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class EspacioEducativo
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.EspacioEducativo.AddObject(this);
            db.SaveChanges();
        }

        public static EspacioEducativo ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            EspacioEducativo ee = new EspacioEducativo();
            var registro = db.EspacioEducativo.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                ee.IdValidarFk = valor;
                ee.Crear();
            }
            ee = db.EspacioEducativo.FirstOrDefault(i => i.IdValidarFk == valor);
            return ee;
        }

        public static EspacioEducativoDet[] ObtenerRegistros(int valor)
        {
            SASEntities db = new SASEntities();
            EspacioEducativoDet[] eed = null;
            var registro = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == valor).Count() > 0 ? true : false;
            if (registro)
            {
                eed = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == valor).ToArray();
            }

            return eed;
        }
    }
}