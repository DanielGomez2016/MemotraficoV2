using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mDepartamentos))]
    public partial class Departamento
    {
        public class mDepartamentos
        {
            public int IdDepartamento { get; set; }
            public string Nombre { get; set; }
            public string Titular { get; set; }
            public string Descripcion { get; set; }
            public string Ext { get; set; }
            [Display(Name = "Institucion")]
            public int IdInstitucionFk { get; set; }

        }

        public int Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Departamento i = db.Departamento.FirstOrDefault(x => x.IdDepartamento == this.IdDepartamento);
                i.Nombre = this.Nombre;
                i.Titular = this.Titular;
                i.Descripcion = this.Descripcion;
                i.Ext = this.Ext;

                db.SaveChanges();
                return i.IdDepartamento;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string getNombre(int? id)
        {
            SASEntities db = new SASEntities();
            if (id != null && id > 0)
            {
                return db.Departamento.FirstOrDefault(i => i.IdDepartamento == id).Nombre;
            }
            else
            {
                return "";
            }
        }
    }
}