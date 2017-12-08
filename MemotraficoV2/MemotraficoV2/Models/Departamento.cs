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

        public static int? getxNombre(string deptorecibe)
        {
            var result = 0;
            var ii = 1;
            try
            {
                switch (deptorecibe)
                {
                    case "DIRECCIÓN GENERAL":
                        result = 5;
                        break;
                    case "DIRECCIÓN JURÍDICO":
                        result = 3;
                        break;
                    case "DIRECCION ADMINISTRATIVA":
                        result = 1;
                        break;
                    case "DIRECCIÓN TÉCNICA":
                        result = 7;
                        break;
                    case "DIRECCION DE PLANEACIÓN":
                        result = 14;
                        break;
                    case "DEPARTAMENTO DE PROYECTOS":
                        result = 15;
                        break;
                    case "DEPARTAMENTO DE VINCULACION SOCIAL E INOVACION TECNOLOGICA":
                        result = 9;
                        break;
                    case "DEPARTAMENTO DE COSTOS":
                        result = 8;
                        break;
                    case "DEPARTAMENTO DE SUPERVISIÓN Y EJECUCION DE OBRA":
                        result = 11;
                        break;
                    case "DEPARTAMENTO DE INFRAESTRUCTURA EDUCATIVA DE CD. JUÁREZ":
                        result = 22;
                        break;
                    case "REHABILITACIÓN, MOBILIARIO Y EQUIPO":
                        result = 13;
                        break;
                    case "DEPARTAMENTO DE LICITACIONES":
                        result = 4;
                        break;
                    case "ESCUELAS AL 100":
                        result = 18;
                        break;
                    case "INIFED":
                        result = 20;
                        ii = 4;
                        break;
                    case "DEPARTAMENTO DE CONTROL TECNICO Y LOGISTICA ":
                        result = 21;
                        break;
                    case "DIRECCIÓN DE OPERACIONES":
                        result = 12;
                        break;
                    case "ASUNTOS GENERALES":
                        result = 14;
                        break;
                    case "INGENIERIA DE PROYECTOS":
                        result = 11;
                        break;
                    case "DELEGACION PARRAL":
                        result = 11;
                        break;
                    case "ESCUELAS AL 100 PLANEACIÓN":
                        result = 18;
                        break;
                }

                return result;
            }
            catch (Exception e)
            {

            }
            return result;
        }
    }
}