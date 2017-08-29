using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mcomponenteVIII))]
    public partial class ComponenteVIII
    {
        public class mcomponenteVIII
        {
            [Required]
            [Display(Name = "Muro Acometida / Red Elect.")]
            public string MuroAcometida { get; set; }

            [Required]
            [Display(Name = "Alumbrado Exterior")]
            public string Alumbrado { get; set; }

            [Required]
            [Display(Name = "Contenedores de Basura")]
            public string ContenedorBasura { get; set; }

            [Required]
            [Display(Name = "Barda / Reja / Malla Perim.")]
            public string Barda { get; set; }

            [Required]
            [Display(Name = "Cancha Multiple / Plaza Civica")]
            public string CanchaMultiple { get; set; }

            [Required]
            [Display(Name = "Domo / Lamina / Malla Sombra")]
            public string Domo { get; set; }

            [Required]
            [Display(Name = "Asta Bandera")]
            public string AstaBandera { get; set; }

            [Required]
            [Display(Name = "Acceso Principal")]
            public string AccesoPrincipal { get; set; }

            [Required]
            [Display(Name = "Area de Juegos Infantiles")]
            public string AreaJuegos { get; set; }

        }
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ComponenteVIII.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ComponenteVIII c8 = db.ComponenteVIII.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            c8.MuroAcometida = MuroAcometida;
            c8.Alumbrado = Alumbrado;
            c8.ContenedorBasura = ContenedorBasura;
            c8.Barda = Barda;
            c8.CanchaMultiple = CanchaMultiple;
            c8.Otros = Otros;
            c8.Domo = Domo;
            c8.AstaBandera = AstaBandera;
            c8.AccesoPrincipal = AccesoPrincipal;
            c8.AreaJuegos = AreaJuegos;
            c8.OtroConcepto = OtroConcepto;
            db.SaveChanges();
        }

        public static ComponenteVIII ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ComponenteVIII c8 = new ComponenteVIII();
            var registro = db.ComponenteVIII.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                c8.IdValidarFk = valor;
                c8.MuroAcometida = "";
                c8.Alumbrado = "";
                c8.ContenedorBasura = "";
                c8.Barda = "";
                c8.CanchaMultiple = "";
                c8.Otros = "";
                c8.Domo = "";
                c8.AstaBandera = "";
                c8.AccesoPrincipal = "";
                c8.AreaJuegos = "";
                c8.OtroConcepto = "";

                c8.Crear();
            }
            c8 = db.ComponenteVIII.FirstOrDefault(i => i.IdValidarFk == valor);
            return c8;
        }
    }
}