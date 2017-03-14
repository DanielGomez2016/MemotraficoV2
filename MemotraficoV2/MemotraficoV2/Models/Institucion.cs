using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Institucion
    {
        public class mInstitucion
        {
            public int IdInstitucion { get; set; }
            public string Nombre { get; set; }
            public string Siglas { get; set; }
            public string Titular { get; set; }
            public string Descripcion { get; set; }
            public string Telefono { get; set; }
            public string Ext { get; set; }
        }

        public int Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Institucion i = db.Institucion.FirstOrDefault(x => x.IdInstitucion == this.IdInstitucion);
                i.Nombre = this.Nombre;
                i.Siglas = this.Siglas;
                i.Titular = this.Titular;
                i.Descripcion = this.Descripcion;
                i.Telefono = this.Telefono;
                i.Ext = this.Ext;

                db.SaveChanges();
                return i.IdInstitucion;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<Institucion> listarPorInstitucionActual()
        {
            SASEntities db = new SASEntities();
            int ins = Usuarios.GetInstitucion();
            return db.Institucion.Where(i => i.IdInstitucion == ins).OrderBy(i => i.Siglas).ToList();
        }

        public static string getnameInstitucion()
        {
            SASEntities db = new SASEntities();
            int ins = Usuarios.GetInstitucion();
            return db.Institucion.FirstOrDefault(i => i.IdInstitucion == ins).Siglas.ToString();
        }

        public static Institucion getinstitucion(int id)
        {
            SASEntities db = new SASEntities();
            return db.Institucion.FirstOrDefault(i => i.IdInstitucion == id);
        }
    }
}