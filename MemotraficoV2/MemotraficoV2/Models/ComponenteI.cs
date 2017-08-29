using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteI))]
    public partial class ComponenteI
    {
        public class mcomponenteI
        {
            [Required]
            [Display(Name = "Costruccion Espacios")]
            public string ConstruccionEspacio { get; set; }

            [Required]
            [Display(Name = "Pintura (m2)")]
            public string Pintura { get; set; }

            [Required]
            [Display(Name = "Impermeabilizante (m2)")]
            public string Impermeabilizante { get; set; }

            [Required]
            [Display(Name = "Piso Ceramica(m2)")]
            public string PisoCeramica { get; set; }

            [Required]
            [Display(Name = "Herreria y Canceleria")]
            public string Herreria { get; set; }

            [Required]
            [Display(Name = "Aire acond / Mini Split")]
            public string AireAcondicionado { get; set; }

            [Required]
            [Display(Name = "Calenton Gas (5 Radianes)")]
            public string CalentonGas { get; set; }

            [Required]
            [Display(Name = "Instalacion Electrica")]
            public string InstElectrica { get; set; }

            [Required]
            [Display(Name = "Inst. HIdro-Sanitaria")]
            public string HidroSanitaria { get; set; }

            [Required]
            [Display(Name = "Inst. de Gas L.P. o Natural")]
            public string InstGas { get; set; }

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteI.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteI c1 = db.ComponenteI.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c1.ConstruccionEspacio = ConstruccionEspacio;
            c1.Pintura = Pintura;
            c1.Impermeabilizante = Impermeabilizante;
            c1.PisoCeramica = PisoCeramica;
            c1.Herreria = Herreria;
            c1.AireAcondicionado = AireAcondicionado;
            c1.CalentonGas = CalentonGas;
            c1.InstElectrica = InstElectrica;
            c1.HidroSanitaria = HidroSanitaria;
            c1.InstGas = InstGas;
            db.SaveChanges();
        }

        public static ComponenteI ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteI c1 = new ComponenteI();
            var registro = db.ComponenteI.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c1.IdValidarFk = valor;
                c1.ConstruccionEspacio = "";
                c1.Pintura = "";
                c1.Impermeabilizante = "";
                c1.PisoCeramica = "";
                c1.Herreria = "";
                c1.AireAcondicionado = "";
                c1.CalentonGas = "";
                c1.InstElectrica = "";
                c1.HidroSanitaria = "";
                c1.InstGas = "";

                c1.Crear();
            }
            c1 = db.ComponenteI.FirstOrDefault(i => i.IdValidarFk == valor);
            return c1;
        }
    }
}