using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteVII))]
    public partial class ComponenteVII
    {
        public class mcomponenteVII
        {
            [Required]
            [Display(Name = "Telefonia / Internet")]
            public string Telefonia_internet { get; set; }

            [Required]
            [Display(Name = " Salida de Voz y Datos")]
            public string VozDatos { get; set; }

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteVII.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteVII c7 = db.ComponenteVII.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c7.Telefonia_internet = Telefonia_internet;
            c7.VozDatos = VozDatos;
            db.SaveChanges();
        }

        public static ComponenteVII ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteVII c7 = new ComponenteVII();
            var registro = db.ComponenteVII.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c7.IdValidarFk = valor;
                c7.Telefonia_internet = "";
                c7.VozDatos = "";

                c7.Crear();
            }
            c7 = db.ComponenteVII.FirstOrDefault(i => i.IdValidarFk == valor);
            return c7;
        }
    }
}