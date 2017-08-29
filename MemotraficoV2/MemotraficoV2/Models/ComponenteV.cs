using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{

    [MetadataType(typeof(mcomponenteV))]
    public partial class ComponenteV
    {
        public class mcomponenteV
        {

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteV.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteV c5 = db.ComponenteV.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c5.Andadores = Andadores;
            c5.Rampas = Rampas;
            c5.Pasamanos = Pasamanos;
            db.SaveChanges();
        }

        public static ComponenteV ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteV c5 = new ComponenteV();
            var registro = db.ComponenteV.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c5.IdValidarFk = valor;
                c5.Andadores = "";
                c5.Rampas = "";
                c5.Pasamanos = "";

                c5.Crear();
            }
            c5 = db.ComponenteV.FirstOrDefault(i => i.IdValidarFk == valor);
            return c5;
        }
    }
}