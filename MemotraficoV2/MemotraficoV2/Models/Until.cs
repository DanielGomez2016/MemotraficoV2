//using Excel;
using MemotraficoV2.Models.Colecciones;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MemotraficoV2.Models
{
    public class Util
    {
        #region Email

        /// <summary>
        /// Enviar un email con template
        /// </summary>
        /// <param name="EmailTo">Destinatario</param>
        /// <param name="TipoAsunto">Subject del email</param>
        /// <param name="template"> Template que utilizara el correo</param>
        /// <param name="parametros">Parametros que recibira el email para ser enviado</param>
        public static void EnviarCorreo(string EmailTo, string TipoAsunto, string template, object parametros)
        {
            /*————————-MENSAJE DE CORREO———————-*/

            //Creamos un nuevo Objeto de mensaje
            MailMessage mmsg = new MailMessage();
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            //Direccion de correo electronico a la que queremos enviar el mensaje
            mmsg.To.Add(EmailTo);

            //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

            //Asunto
            mmsg.Subject = TipoAsunto;
            mmsg.SubjectEncoding = Encoding.UTF8;

            //Direccion de correo electronico que queremos que reciba una copia del mensaje
            //mmsg.Bcc.Add(appConfig["EmailReplica"]); //Opcional

            //Cuerpo del Mensaje
            mmsg.Body = CrearFormato(template, parametros);
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.IsBodyHtml = true; //Si no queremos que se envíe como HTML

            //Correo electronico desde la que enviamos el mensaje
            mmsg.From = new MailAddress(appConfig["EmailUser"]);

            /*————————-CLIENTE DE CORREO———————-*/

            //Creamos un objeto de cliente de correo
            SmtpClient cliente = new SmtpClient();

            //Hay que crear las credenciales del correo emisor
            cliente.Credentials =
            new NetworkCredential(appConfig["EmailUser"], appConfig["EmailPass"]);

            //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail

            cliente.Port = int.Parse(appConfig["EmailPort"]); 
            cliente.EnableSsl = true;


            cliente.Host = appConfig["EmailHost"]; //gobierno del estado;

            /*————————-ENVIO DE CORREO———————-*/

            try
            {
                //Enviamos el mensaje
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                //Aquí gestionamos los errores al intentar enviar el correo
            }
        }

        /// <summary>
        /// Enviar en email utilizando un template html alcamenado en el servidor
        /// </summary>
        /// <param name="template">Nombre del archivo</param>
        /// <param name="parametros">Datos que tendrá el email</param>
        public static string CrearFormato(string template, object parametros) {

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/PlantillasEmail/");

            if (!string.IsNullOrEmpty(template))
            {
                try
                {
                    string html = File.ReadAllText(path + template);

                    if (parametros != null)
                    {
                        foreach (var m in parametros.GetType().GetProperties())
                        {
                            html = html.Replace("{" + m.Name + "}", parametros.GetType().GetProperty(m.Name).GetValue(parametros, null).ToString());
                        }
                    }


                    return html;
                }
                catch (Exception e)
                {
                    
                }
            }
            return "";
        }

        /// <summary>
        /// Guardar nueva notificacion en la tabla de email
        /// </summary>
        /// <param name="email">email a quien se le enviara la notificacion</param>
        /// <param name="titulo">titulo que llevara el correo</param>
        /// <param name="msj">plantilla que se utilizara en el correo</param>
        /// <param name="user">Usuario que realizo el registro</param>
        /// <param name="controlador">nombre de controlador para acceder a los datos del mensaje</param>
        /// <param name="indice">id para ingresar a los datos</param>
        public static void IngresarNotificacion(string email, string titulo, string msj, string user, string controlador, int indice)
        {
            SASEntities db = new SASEntities();
            Email e = new Email();
            e.EmailTo = email;
            e.Subject = titulo;
            e.Message = msj;
            e.IdUser = user;
            e.Controlador = controlador;
            e.Indice = indice;
            e.Status = ListaStatusEmail.ENVIAR;

            db.Email.AddObject(e);
            db.SaveChanges();
        }

        public static int getEstatus(int idestatus, bool cerrado, bool cancelado, int nocanalizacion, int totalcanalizacion)
        {
            var result = 0;

            try
            {
                if (nocanalizacion == totalcanalizacion)
                {
                    if (cerrado) { result = ListaEstatus.CERRADO; }
                    if (cancelado) { result = ListaEstatus.CANCELADO; }
                }
                else { result = ListaEstatus.CANALIZADO; }

            }
            catch (Exception e) {

            }

            return result;
        }

        #endregion
    }
}