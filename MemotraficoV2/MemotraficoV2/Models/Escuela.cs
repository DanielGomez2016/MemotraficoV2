using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mEscuela))]
    public partial class Escuela
    {
        public class mEscuela
        {
            [Required]
            [Display(Name ="Plantel Educativo")]
            public string Nombre { get; set; }

            [Display(Name = "Municipio")]
            public string IdMunicipioFk { get; set; }

            [Display(Name = "Localidad")]
            public string IdLocalidadFk { get; set; }

            [Display(Name = "Nivel Educativo")]
            public string IdNivelEducativo { get; set; }

            [Display(Name = "Geolocalizacion X")]
            public string Geox { get; set; }

            [Display(Name = "Geolocalizacion Y")]
            public string Geoy { get; set; }
        }

        [Display(Name = "Nombre Director")]
        public string NombreDirector { get; set; }

        [Display(Name = "Email Director")]
        public string EmailDirector { get; set; }

        public string Telefono { get; set; }
        public string Celular { get; set; }

        public int Crear()
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Escuela.AddObject(this);
                db.SaveChanges();

                return IdEscuela;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Escuela e = db.Escuela.FirstOrDefault(i => i.IdEscuela == this.IdEscuela);
                e.Nombre = Nombre != null ? Nombre : e.Nombre;
                e.Clave = Clave != null ? Clave : e.Clave;
                e.Domicilio = Domicilio != null ? Domicilio : e.Domicilio;
                e.IdLocalidadFk = IdLocalidadFk != null ? IdLocalidadFk : e.IdLocalidadFk;
                e.IdMunicipioFk = IdMunicipioFk != null ? IdMunicipioFk : e.IdMunicipioFk;
                e.IdNivelEducativo = IdNivelEducativo != null ? IdNivelEducativo : e.IdNivelEducativo;
                e.Turno = Turno != null ? Turno : e.Turno;
                e.Geox = Geox != null ? Geox : e.Geox;
                e.Geoy = Geoy != null ? Geoy : e.Geoy;

                db.SaveChanges();

                return IdEscuela;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int EditarNivel()
        {
            try
            {
                SASEntities db = new SASEntities();
                Escuela e = db.Escuela.FirstOrDefault(i => i.Clave == this.Clave);
                e.IdNivelEducativo = IdNivelEducativo != null ? IdNivelEducativo : e.IdNivelEducativo;
                db.SaveChanges();

                return IdEscuela;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }


}