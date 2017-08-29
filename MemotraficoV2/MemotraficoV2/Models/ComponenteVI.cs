using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteVI))]
    public partial class ComponenteVI
    {
        public class mcomponenteVI
        {
            [Required]
            [Display(Name = "Construccion de Modulo")]
            public string ConstruccionModulo { get; set; }

            [Required]
            [Display(Name = "Pintura (m2)")]
            public string Pintura { get; set; }

            [Required]
            [Display(Name = "Piso de Ceramica (m2)")]
            public string Piso { get; set; }

            [Required]
            [Display(Name = "Instalacion Electrica")]
            public string InstalacionElectrica { get; set; }

            [Required]
            [Display(Name = "Impermeabilizante (m2)")]
            public string Impermeabilizante { get; set; }

            [Required]
            [Display(Name = "Aires Acond. / Calenton")]
            public string AireAcondicionado { get; set; }


        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteVI.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteVI c6 = db.ComponenteVI.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c6.ConstruccionModulo = ConstruccionModulo;
            c6.Pintura = Pintura;
            c6.Piso = Piso;
            c6.InstalacionElectrica = InstalacionElectrica;
            c6.AireAcondicionado = AireAcondicionado;
            c6.Impermeabilizante = Impermeabilizante;
            db.SaveChanges();
        }

        public static ComponenteVI ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteVI c6 = new ComponenteVI();
            var registro = db.ComponenteVI.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c6.IdValidarFk = valor;
                c6.ConstruccionModulo = "";
                c6.Pintura = "";
                c6.Piso = "";
                c6.InstalacionElectrica = "";
                c6.AireAcondicionado = "";
                c6.Impermeabilizante = "";

                c6.Crear();
            }
            c6 = db.ComponenteVI.FirstOrDefault(i => i.IdValidarFk == valor);
            return c6;
        }
    }
}