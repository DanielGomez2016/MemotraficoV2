using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Escuela
    {
        public string NombreDirector { get; set; }
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
                e.Nombre = Nombre;
                e.Clave = Clave;
                e.Domicilio = Domicilio;
                e.IdLocalidadFk = IdLocalidadFk;
                e.IdMunicipioFk = IdMunicipioFk;
                e.IdNivelEducativo = IdNivelEducativo;
                e.Turno = Turno;
                e.Geox = Geox;
                e.Geoy = Geoy;

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