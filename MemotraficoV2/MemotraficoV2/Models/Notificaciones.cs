using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using MemotraficoV2.Models;

namespace MemotraficoV2.Models
{
    public class Notificaciones : Registry
    {
        
        public Notificaciones()
        {
            Schedule(() =>
            {
                SASEntities db = new SASEntities();
                IQueryable<Email> emails = db.Email.Where(i => i.Status == "Enviar");
                if (emails.Any())
                {
                    foreach (var email in emails)
                    {
                        EnviarNotificaciones(email.EmailTo, email.Subject, email.Message, email.IdEmail);
                    }
                }
            }).ToRunNow().AndEvery(1).Hours();//.AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);
        }

        public void EnviarNotificaciones(string EmailTo, string TipoAsunto, string formato, int IdEmail)
        {
            Util.EnviarCorreo(EmailTo,TipoAsunto,formato,Obj(IdEmail));

            SASEntities db = new SASEntities();
            Email email = db.Email.FirstOrDefault(i => i.IdEmail == IdEmail);
            email.Status = "Enviado";

            db.SaveChanges();

        }

        public object Obj(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                Email email = db.Email.FirstOrDefault(x => x.IdEmail == id);

                switch (email.Controlador)
                {
                    case "Solicitudes":

                        var solicitud = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var contacto = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == solicitud.IdEscuelaFk);

                        return new {
                            Titulo = email.Subject,
                            Fecha = solicitud.FechaEntrega.ToString(),
                            Manager = contacto.Nombre,
                            Instituto = solicitud.Escuela.Nombre,
                            Folio = solicitud.Folio,
                            Url = "SAS/Seguimiento/?folio=" + solicitud.Folio,//cambiar la direccion a donde va dirijido
                            UnidadAdmin = "Secretaria de Educación Publica",
                            Telefono = "(614) 4566-8695",
                            Ext = "25461"
                        };

                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}