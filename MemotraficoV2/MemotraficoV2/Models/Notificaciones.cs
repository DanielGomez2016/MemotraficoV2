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
using System.Globalization;

namespace MemotraficoV2.Models
{
    public class Notificaciones : Registry
    {
        public class DatosInstituto
        {
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string Extension { get; set; }

        }
        
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
                    #region Nueva solicitud
                    case "Solicitudes":

                        var solicitud = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var contacto = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == solicitud.IdEscuelaFk);

                        return new {
                            Titulo = email.Subject,
                            Fecha = Fecha(solicitud.FechaEntrega.Value),
                            Manager = contacto.Nombre,
                            Instituto = solicitud.Escuela.Nombre,
                            Folio = solicitud.Folio,
                            Url = "SAS/Seguimiento/?folio=" + solicitud.Folio,//cambiar la direccion a donde va dirijido
                            UnidadAdmin = "Secretaria de Educación Publica",
                            Telefono = "(614) 4566-8695",
                            Ext = "25461"
                        };
                    #endregion

                    #region Canalizacion para solicitante

                    case "CanalizacionSolicitante":

                        var scs = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var ccs = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == scs.IdEscuelaFk);
                        var canalizacion = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == scs.IdSolicitud).IdCanalizacion;
                        var dccs = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion);

                        DatosInstituto datos = datosInstituto(dccs.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(scs.FechaEntrega.Value),
                            Manager = ccs.Nombre,
                            Instituto = scs.Escuela.Nombre,
                            Folio = scs.Folio,
                            Url = "SAS/Seguimiento/?folio=" + scs.Folio,//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos.Nombre,
                            Telefono = datos.Telefono,
                            Ext = datos.Extension,
                            Direccion = datos.Direccion,
                            Responsable = dccs.UsuarioAtiende != null ? NombreAtiende(dccs.UsuarioAtiende) : " ",
                            Comentario = dccs.Comentario,
                            Inicio = Fecha(dccs.FechaCanalizar),
                            Fin = "",
                        };

                    #endregion

                    #region Canalizacion para Sitema

                    case "CanalizacionSistema":

                        var scsi = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var ccsi = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == scsi.IdEscuelaFk);
                        var canalizacion2 = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == scsi.IdSolicitud).IdCanalizacion;
                        var dccsi = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion2);

                        DatosInstituto datos2 = datosInstituto(dccsi.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(scsi.FechaEntrega.Value),
                            Manager = ccsi.Nombre,
                            Instituto = scsi.Escuela.Nombre,
                            Folio = scsi.Folio,
                            Url = "SAS/Solicitudes/" ,//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos2.Nombre,
                            Telefono = datos2.Telefono,
                            Ext = datos2.Extension,
                            Direccion = datos2.Direccion,
                            Responsable = dccsi.UsuarioAtiende != null ? NombreAtiende(dccsi.UsuarioAtiende) : " ",
                            Comentario = dccsi.Comentario,
                            Inicio = Fecha(dccsi.FechaCanalizar),
                            Fin = "",
                        };

                    #endregion

                    #region Cancelacion para solicitante

                    case "CancelacionSolicitante":

                        var scas = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var ccas = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == scas.IdEscuelaFk);
                        var canalizacion3 = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == scas.IdSolicitud).IdCanalizacion;
                        var dccas = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion3);

                        DatosInstituto datos3 = datosInstituto(dccas.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(scas.FechaEntrega.Value),
                            Manager = ccas.Nombre,
                            Instituto = scas.Escuela.Nombre,
                            Folio = scas.Folio,
                            Url = "SAS/Solicitudes/",//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos3.Nombre,
                            Telefono = datos3.Telefono,
                            Ext = datos3.Extension,
                            Direccion = datos3.Direccion,
                            Responsable = dccas.UsuarioAtiende != null ? NombreAtiende(dccas.UsuarioAtiende) : " ",
                            Comentario = dccas.Comentario,
                            Inicio = "",
                            Fin = Fecha(dccas.FechaCanalizar),
                        };

                    #endregion

                    #region Cancelacion para sistema

                    case "CancelacionSistema":

                        var scasi = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var ccasi = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == scasi.IdEscuelaFk);
                        var canalizacion4 = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == scasi.IdSolicitud).IdCanalizacion;
                        var dccasi = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion4);

                        DatosInstituto datos4 = datosInstituto(dccasi.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(scasi.FechaEntrega.Value),
                            Manager = ccasi.Nombre,
                            Instituto = scasi.Escuela.Nombre,
                            Folio = scasi.Folio,
                            Url = "SAS/Solicitudes/",//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos4.Nombre,
                            Telefono = datos4.Telefono,
                            Ext = datos4.Extension,
                            Direccion = datos4.Direccion,
                            Responsable = dccasi.UsuarioAtiende != null ? NombreAtiende(dccasi.UsuarioAtiende) : " ",
                            Comentario = dccasi.Comentario,
                            Inicio = "",
                            Fin = Fecha(dccasi.FechaCanalizar),
                        };

                    #endregion

                    #region Atendida para solicitante

                    case "AtendidaSolicitante":

                        var sats = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var cats = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == sats.IdEscuelaFk);
                        var canalizacion5 = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == sats.IdSolicitud).IdCanalizacion;
                        var dcats = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion5);

                        DatosInstituto datos5 = datosInstituto(dcats.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(sats.FechaEntrega.Value),
                            Manager = cats.Nombre,
                            Instituto = sats.Escuela.Nombre,
                            Folio = sats.Folio,
                            Url = "SAS/Solicitudes/",//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos5.Nombre,
                            Telefono = datos5.Telefono,
                            Ext = datos5.Extension,
                            Direccion = datos5.Direccion,
                            Responsable = dcats.UsuarioAtiende != null ? NombreAtiende(dcats.UsuarioAtiende) : " ",
                            Comentario = dcats.Comentario,
                            Inicio = "",
                            Fin = Fecha(dcats.FechaCanalizar),
                        };

                    #endregion

                    #region Atendida para sistema

                    case "AtendidaSistema":

                        var satsi = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == email.Indice);
                        var catsi = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == satsi.IdEscuelaFk);
                        var canalizacion6 = db.Canalizacion.FirstOrDefault(z => z.IdSolicitudFk == satsi.IdSolicitud).IdCanalizacion;
                        var dcatsi = db.DetalleCanalizacion
                                   .OrderByDescending(x => x.FechaCanalizar)
                                   .FirstOrDefault(x => x.IdCanalizarFk == canalizacion6);

                        DatosInstituto datos6 = datosInstituto(dcatsi.Instituto);

                        return new
                        {
                            Titulo = email.Subject,
                            Fecha = Fecha(satsi.FechaEntrega.Value),
                            Manager = catsi.Nombre,
                            Instituto = satsi.Escuela.Nombre,
                            Folio = satsi.Folio,
                            Url = "SAS/Solicitudes/",//cambiar la direccion a donde va dirijido
                            UnidadAdmin = datos6.Nombre,
                            Telefono = datos6.Telefono,
                            Ext = datos6.Extension,
                            Direccion = datos6.Direccion,
                            Responsable = dcatsi.UsuarioAtiende != null ? NombreAtiende(dcatsi.UsuarioAtiende) : " ",
                            Comentario = dcatsi.Comentario,
                            Inicio = "",
                            Fin = Fecha(dcatsi.FechaCanalizar),
                        };

                        #endregion

                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private DatosInstituto datosInstituto(int? instituto)
        {
            SASEntities db = new SASEntities();
            DatosInstituto di = new DatosInstituto();

            var i = db.Institucion.FirstOrDefault(x => x.IdInstitucion == instituto);
            var dir = "";
            var tel = "";
            var ext = "";
            switch (i.Siglas)
            {
                case "ICHIFE":
                    dir = "Av. Río de Janeiro No. 1000<br />"+
                          "Residencial El Campestre, C.P. 31238 <br />"+
                          "Chihuahua, Chih., México <br /> ";
                    tel = "(614) 123-4587";
                    ext = "23546";
                    break;

                case "SEECH":
                    dir = "Av. Río de Janeiro No. 1000<br />" +
                          "Residencial El Campestre, C.P. 31238 <br />" +
                          "Chihuahua, Chih., México < br /> ";
                    tel = "(614) 123-4587";
                    ext = "23546";
                    break;

                case "SEyD":
                    dir = "Av. Río de Janeiro No. 1000<br />" +
                          "Residencial El Campestre, C.P. 31238 <br />" +
                          "Chihuahua, Chih., México <br /> ";
                    tel = "(614) 123-4587";
                    ext = "23546";
                    break;
            }

            return new DatosInstituto
            {
                Nombre = i.Nombre,
                Direccion = dir,
                Telefono = tel,
                Extension = ext
            };
        }

        public object NombreAtiende(string ua)
        {
            SASEntities db = new SASEntities();
            var n = db.AspNetUsers.FirstOrDefault(x => x.Id == ua);
            return n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno;
        }

        public string Fecha(DateTime? dt)
        {
            var day = dt.Value.Day;
            var month = dt.Value.Month;
            var year = dt.Value.Year;

            var fecha = "";

            fecha += day.ToString() + " de ";

            switch (month)
            {
                case 1:
                    fecha += "Enero del ";
                    break;
                case 2:
                    fecha += "Febrero del ";
                    break;
                case 3:
                    fecha += "Marzo del ";
                    break;
                case 4:
                    fecha += "Abril del ";
                    break;
                case 5:
                    fecha += "Mayo del ";
                    break;
                case 6:
                    fecha += "Junio del ";
                    break;
                case 7:
                    fecha += "Julio del ";
                    break;
                case 8:
                    fecha += "Agosto del ";
                    break;
                case 9:
                    fecha += "Septiembre del ";
                    break;
                case 10:
                    fecha += "Octubre del ";
                    break;
                case 11:
                    fecha += "Noviembre del ";
                    break;
                case 12:
                    fecha += "Diciembre del ";
                    break;
            }

            fecha += year.ToString();

            return fecha;
        }

    }
}