using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteIV))]
    public partial class ComponenteIV
    {
        public class mcomponenteIV
        {
            [Required]
            [Display(Name = "Red Hidraulica")]
            public string RedHidraulica { get; set; }

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteIV.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteIV c4 = db.ComponenteIV.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c4.RedHidraulica = RedHidraulica;
            c4.Construccion = Construccion;
            c4.Rehabilitacion = Rehabilitacion;
            db.SaveChanges();
        }

        public static ComponenteIV ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteIV c4 = new ComponenteIV();
            var registro = db.ComponenteIV.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c4.IdValidarFk = valor;
                c4.RedHidraulica = "";
                c4.Construccion = "";
                c4.Rehabilitacion = 0;

                c4.Crear();
            }
            c4 = db.ComponenteIV.FirstOrDefault(i => i.IdValidarFk == valor);
            return c4;
        }
    }
}