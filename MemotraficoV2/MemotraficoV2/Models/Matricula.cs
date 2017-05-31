using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mMatricula))]
    public partial class Matricula
    {
        public class mMatricula
        {
            [Required]
            [Display(Name = "Personas Con Disc.")]
            public string PersonaDiscapacidad { get; set; }

            [Required]
            [Display(Name = "Personal Docente")]
            public string PersonalDoncente { get; set; }

        }
        public int Total { get; set; }
        public void Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Matricula m = db.Matricula.FirstOrDefault(i => i.IdMatricula == IdMatricula);
                m.PersonaDiscapacidad = PersonaDiscapacidad != null ? PersonaDiscapacidad : m.PersonaDiscapacidad;
                m.PersonalDoncente = PersonalDoncente != null ? PersonalDoncente : m.PersonalDoncente;
                m.Alumnos = Alumnos != null ? Alumnos : m.Alumnos;
                db.SaveChanges();

            }catch(Exception e)
            {
            }
        }
        public void Crear()
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Matricula.AddObject(this);
                db.SaveChanges();

            }
            catch (Exception e)
            {

            }
        }

        public static Matricula ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            Matricula m = new Matricula();
            var registro = db.Matricula.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro) {
                m.IdValidarFk = valor;
                m.Alumnos = 0;
                m.PersonaDiscapacidad = 0;
                m.PersonalDoncente = 0;
                m.Crear();
            }
            m = db.Matricula.FirstOrDefault(i => i.IdValidarFk == valor);
            m.Total = Convert.ToInt32(m.Alumnos + m.PersonaDiscapacidad);
            return m;
        }
    }
}