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

        public void Editar()
        {
            SASEntities db = new SASEntities();
        }
    }
}