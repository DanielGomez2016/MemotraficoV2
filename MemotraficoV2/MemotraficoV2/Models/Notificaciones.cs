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
                foreach (var email in emails)
                {
                    var TipoCorreo = email.TipoEmail;
                    String Cuerpo = string.Format("Se notifica a la persona (nombre persona o institucion, que su solicitud a sido (cancelada,promovida,cerrada,atendida) por la persona a cargo de la solicitud siendo (nombre encargado) con telefono (telefono) Ext. (extencion), de la institucion correspondiente (nombre instutucion) con la siguiente descripcion de la solicitud: (descripcion))");
                    EnviarNotificaciones(TipoCorreo, Cuerpo, email.EmailTo, email.Subject ,email.IdEmail);
                }

            }).ToRunNow().AndEvery(1).Hours();//.AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);
        }

        public void EnviarNotificaciones(string TipoCorreo, string cuerpo, string correo, string asuntoCorreo, int idemail)
        {
            Util.SendEmail(TipoCorreo , correo, cuerpo, asuntoCorreo);

            SASEntities db = new SASEntities();
            Email email = db.Email.FirstOrDefault(i => i.IdEmail == idemail);
            email.Status = "Enviado";

            db.SaveChanges();

        }
    }
}