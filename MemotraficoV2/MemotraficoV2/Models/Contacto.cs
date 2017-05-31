using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Contacto
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.Contacto.AddObject(this);
            db.SaveChanges();

        }
        public void Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Contacto c = db.Contacto.FirstOrDefault(i => i.IdContacto == this.IdContacto);
                c.Nombre = Nombre;
                c.Email = Email;
                c.Telefono = Telefono;
                c.Celular = Celular;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}