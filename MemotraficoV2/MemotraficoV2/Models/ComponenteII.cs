using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteII))]
    public partial class ComponenteII
    {
        public class mcomponenteII
        {
            [Required]
            [Display(Name = "Costruccion de Modulo")]
            public string ConstruccionModulo { get; set; }

            [Required]
            [Display(Name = "Pintura (m2)")]
            public string Pintura { get; set; }

            [Required]
            [Display(Name = "Muebles Sanitarios")]
            public string MuebleSanitario { get; set; }

            [Required]
            [Display(Name = "Piso  / Lambirn Ceramico(m2)")]
            public string Piso { get; set; }

            [Required]
            [Display(Name = "Herreria y Canceleria")]
            public string Herreria { get; set; }

            [Required]
            [Display(Name = "Fosa Septica / Biodegestor")]
            public string FosaSeptica { get; set; }

            [Required]
            [Display(Name = "Cisterna / Tinacos")]
            public string Cisterna { get; set; }

            [Required]
            [Display(Name = "Instalacion Electrica")]
            public string InstalacionElectrica { get; set; }

            [Required]
            [Display(Name = "Inst. HIdro-Sanitaria")]
            public string HidroSanitaria { get; set; }

            [Required]
            [Display(Name = "Impermeabilizante (m2)")]
            public string Impermeabilizante { get; set; }

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteII.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteII c2 = db.ComponenteII.FirstOrDefault(i => i.IdRequerimientoFk == IdRequerimientoFk);
            c2.ConstruccionModulo = ConstruccionModulo;
            c2.MuebleSanitario = MuebleSanitario;
            c2.Pintura = Pintura;
            c2.Piso = Piso;
            c2.Herreria = Herreria;
            c2.FosaSeptica = FosaSeptica;
            c2.Cisterna = Cisterna;
            c2.InstalacionElectrica = InstalacionElectrica;
            c2.HidroSanitaria = HidroSanitaria;
            c2.Impermeabilizante = Impermeabilizante;
            db.SaveChanges();
        }

        public static ComponenteII ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteII c2 = new ComponenteII();
            var registro = db.ComponenteII.Where(i => i.IdRequerimientoFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c2.IdRequerimientoFk = valor;
                c2.ConstruccionModulo = "";
                c2.MuebleSanitario = "";
                c2.Pintura = "";
                c2.Piso = "";
                c2.Herreria = "";
                c2.FosaSeptica = "";
                c2.Cisterna = "";
                c2.InstalacionElectrica = "";
                c2.HidroSanitaria = "";
                c2.Impermeabilizante = "";

                c2.Crear();
            }
            c2 = db.ComponenteII.FirstOrDefault(i => i.IdRequerimientoFk == valor);
            return c2;
        }
    }
}