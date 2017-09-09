using MemotraficoV2.Models.Colecciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Canalizacion
    {
        public int Crear()
        {
            SASEntities db = new SASEntities();
            db.Canalizacion.AddObject(this);
            db.SaveChanges();

            return IdCanalizacion;
        }

        public int Editar()
        {
            SASEntities db = new SASEntities();
            Canalizacion c = db.Canalizacion.FirstOrDefault(j => j.IdCanalizacion == IdCanalizacion);
            c.IdInstitucionFk = IdInstitucionFk;
            db.SaveChanges();

            return IdCanalizacion;
        }

        public void EditarValidacion()
        {
            SASEntities db = new SASEntities();
            Canalizacion c = db.Canalizacion.FirstOrDefault(j => j.IdCanalizacion == IdCanalizacion);
            c.Validacion = Validacion;
            db.SaveChanges();
        }

        //es la canalizacion generica que siempre se realiza cuando una solicitud se va a asignar a algun usuario
        public static int Canalizar(int s, int i, int d, string comentario, string usuarioasigna, int estatus)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            int canaliza = 0;
            int canalizaDet = 0;
            var rol = Usuarios.Roles();

            //si existe una primera canalizacion se edita la canalizacion y se inserta un nuevo detalle, sino se realiza todo de cero
            if (db.Canalizacion.Any(j => j.IdSolicitudFk == s))
            {
                c = db.Canalizacion.FirstOrDefault(k => k.IdSolicitudFk == s);
                if (i > 0)
                {
                    c.IdInstitucionFk = i;
                }
                canaliza = c.Editar();

                //el id de canalizacion para seguir con el historial
                dc.IdCanalizarFk = canaliza;

                //fecha en que se realizo la canalizacion
                dc.FechaCanalizar = DateTime.Now;

                //se asigna el comentario por el motivo que se esta haciendo la canalizacion dependiendo del rol que tenga se agrega un plus al comentario, en dado caso que el comentario estuviera vacio se agrega uno por default
                if(comentario != "" || comentario =="") {
                    switch (rol)
                    {
                        case "Administrador de Solicitudes":
                            dc.Comentario = comentario != "" ? comentario : ListaComentarios.CanalizadaDependencia.ToString(); ;
                            break;
                        case "Administrador de Dependencia":
                            dc.Comentario = comentario != "" ? comentario : ListaComentarios.CanalizadaOperador.ToString(); ;
                            break;
                        case "Operador":
                            dc.Comentario = comentario != "" ? comentario : ListaComentarios.CanalizadaOxO.ToString(); ;
                            break;
                    }
                }
                
                //usuario que esta haciendo la canalizacion de la solicitud
                dc.IdUsuarioFk = Usuarios.GetUsuario();

                //se asigna el instituto que sera por default sobre el usuario que este asiendo la canalizacion a menos que venga de administrador de solicitudes
                if (i > 0)
                {
                    dc.Instituto = i;
                }
                else
                {
                    dc.Instituto = Usuarios.GetInstitucion();
                }

                //asignar al departamento correspondiente de la institucion por medio del administrador de dependencia
                if (d > 0)
                {
                    dc.Departamento = d;
                }

                //asignar al operador correspondiente del departamento por medio del administrador de dependencia
                if (usuarioasigna != "")
                {
                    dc.UsuarioAtiende = usuarioasigna;
                }
                if(usuarioasigna == "" && rol != ListaRoles.ADMINISTRADOR_SOLICITUDES)
                {
                    dc.UsuarioAtiende = Usuarios.GetUsuario();
                }

                //se asigna un estatus al detalle para contunuar con el hjistorial
                if(estatus > 0)
                {
                    dc.IdEstatusFk = estatus;
                }
                
                //se crea un detalle mas para el historial de la solicitud
                canalizaDet = dc.Crear();
            }
            else
            {
                c.IdInstitucionFk = i;
                c.Validacion = Convert.ToBoolean(ListaValidaciones.NO_VALIDACION);
                c.IdSolicitudFk = s;
                canaliza = c.Crear();

                //se crea la primera historia de la solicitud
                dc.IdCanalizarFk = canaliza;
                dc.FechaCanalizar = DateTime.Now;
                dc.Comentario = comentario != "" ? ". " : " " + ListaComentarios.INICIADA;
                dc.IdUsuarioFk = Usuarios.GetUsuario();
                dc.Departamento = 0;
                dc.Instituto = 0;
                dc.IdEstatusFk = estatus;
                canalizaDet = dc.Crear();
            }

            TipoNotificacion(estatus,s);

            return canalizaDet;
        }

        //se utiliza para cancelar las solicitudes
        public static void Cancelacion(int s, int i,int d, string comentario, int estatus)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            //se crea un historial de cancelacion de la solicitud en detallesolicitud
            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;
            comentario = comentario != "" ? comentario : ListaComentarios.Cancelacion.ToString(); ;

            dc.Comentario = comentario;
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = d;
            dc.Instituto = i;
            dc.IdEstatusFk = estatus;
            dc.Crear();

            TipoNotificacion(estatus, s);
        }

        //se utiliza para abrir las solicitudes que estan canceladas
        public static void canalizarSimple(int s, string comentario, int estatus)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            //se crea un historial de cancelacion de la solicitud en detallesolicitud
            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;
            comentario = comentario != "" ? comentario : ListaComentarios.ReAbrir.ToString(); ;

            dc.Comentario = comentario;
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = estatus;
            dc.Crear();

            TipoNotificacion(estatus, s);
        }

        //se utiliza para crear un historial de algun avance en una solicitud, por lo general solo lo usara el rol de operador
        public static int CanalizarAvance(int s, string comentario)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            Documentos doc = new Documentos();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            //se crea un historial de canalizacion sobre un avance que se realizo de la solicitud
            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;
            comentario = comentario != "" ? comentario : ListaComentarios.Avance.ToString(); ;

            dc.Comentario = comentario;
            dc.IdUsuarioFk = Usuarios.RolAlto(Usuarios.Roles(), Usuarios.GetInstitucion());
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = ListaEstatus.CANALIZADO;
            dc.UsuarioAtiende = Usuarios.GetUsuario();

            return dc.Crear();
        }

        //Se utiliza para canalizar directamente a ichife
        public static int CanalizarIchife(int s, string comentario)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            Documentos doc = new Documentos();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;

            dc.Comentario = comentario;
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = ListaEstatus.CANALIZADO;
            dc.UsuarioAtiende = Usuarios.GetUsuario();

            TipoNotificacion(ListaEstatus.CANALIZADO, s);

            return dc.Crear();

        }

        //canaliza las solicitudes que ya se atendieron y el administrador de solicitudes pueda cerrar la solicitud,tiene que pasar por los tres niveles operador,dependencia,admin solicitudes, para dar como concluida la solicitud
        public static int CanalizarAtendida(int s, string comentario)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            Documentos doc = new Documentos();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            //se crea un historial de canalizacion sobre la atencion que se realizo de la solicitud
            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;
            dc.Comentario = comentario != "" ? comentario : ListaComentarios.Atendida.ToString();
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = ListaEstatus.ATENDIDA;
            dc.UsuarioAtiende = Usuarios.RolAlto(Usuarios.Roles(), Usuarios.GetInstitucion());

            TipoNotificacion(ListaEstatus.ATENDIDA, s);

            return dc.Crear();
        }

        //cierra la solicitud que ya fue atendida solo por el rol de administrador de solicitudes
        public static int CerrarSolicitud(int s, string comentario)
        {
            SASEntities db = new SASEntities();
            Canalizacion c = new Canalizacion();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            Documentos doc = new Documentos();

            c = db.Canalizacion.FirstOrDefault(j => j.IdSolicitudFk == s);

            //se crea un historial de canalizacion sobre el cierre que se realizo a la solicitud
            dc.IdCanalizarFk = c.IdCanalizacion;
            dc.FechaCanalizar = DateTime.Now;
            dc.Comentario = comentario != "" ? comentario : ListaComentarios.Cerrada.ToString();
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = 0;
            dc.Instituto = 0;
            dc.IdEstatusFk = ListaEstatus.CERRADO;

            TipoNotificacion(ListaEstatus.CERRADO, s);

            return dc.Crear();
        }

        public static void TipoNotificacion(int status, int s)
        {
            SASEntities db = new SASEntities();

            var emailEscuela = getEmailEscuela(s);
            var emailUsuarioAS = Usuarios.GetEmailAS();

            var canalizacion = db.Canalizacion.FirstOrDefault(x => x.IdSolicitudFk == s).IdCanalizacion;
            var dc = db.DetalleCanalizacion.OrderByDescending(x => x.FechaCanalizar).FirstOrDefault(x => x.IdCanalizarFk == canalizacion).UsuarioAtiende;


            switch (status)
            {
                case ListaEstatus.INICIADO:

                    Util.IngresarNotificacion(emailEscuela, "Nueva Solicitud", "NuevaSolicitud.html", Usuarios.GetUsuario(), "Solicitudes", s);
                    foreach (var i in emailUsuarioAS) { 
                        Util.IngresarNotificacion(i, "Nueva Solicitud", "NuevaSolicitud.html", Usuarios.GetUsuario(), "Solicitudes", s);
                    }

                    break;
                case ListaEstatus.CANALIZADO:

                    Util.IngresarNotificacion(emailEscuela, "Solicitud Canalizada", "CanalizacionSolicitud.html", Usuarios.GetUsuario(), "CanalizacionSolicitante", s);
                    Util.IngresarNotificacion(Usuarios.GetEmail(dc), "Solicitud Canalizada", "CanalizacionSolicitud.html", Usuarios.GetUsuario(), "CanalizacionSistema", s);

                    break;
                case ListaEstatus.CANCELADO:

                    Util.IngresarNotificacion(emailEscuela, "Solicitud Cancelada", "CancelarSolicitud.html", Usuarios.GetUsuario(), "CancelacionSolicitante", s);
                    if(dc != null || dc != "") {
                        Util.IngresarNotificacion(Usuarios.GetEmail(dc), "Solicitud Cancelada", "CancelarSolicitud.html", Usuarios.GetUsuario(), "CancelacionSistema", s);
                    }
                    else {
                        Util.IngresarNotificacion(getEmailUsuario(), "Solicitud Cancelada", "CancelarSolicitud.html", Usuarios.GetUsuario(), "CancelacionSistema", s);
                    }

                    break;
                case ListaEstatus.ATENDIDA:

                    Util.IngresarNotificacion(emailEscuela, "Solicitud Atendida", "AtiendeSolicitud.html", Usuarios.GetUsuario(), "AtendidaSolicitante", s);
                    Util.IngresarNotificacion(Usuarios.GetEmail(dc), "Solicitud Atendida", "AtiendeSolicitud.html", Usuarios.GetUsuario(), "AtendidaSistema", s);

                    break;
            }
        }

        public static string getEmailEscuela(int solicitud)
        {
            SASEntities db = new SASEntities();
            var s = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == solicitud).IdEscuelaFk;
            var c = db.Contacto.FirstOrDefault(x => x.IdEscuelaFk == s).Email;

            return c;
        }

        public static string getEmailUsuario()
        {
            SASEntities db = new SASEntities();
            var e = db.AspNetUsers.FirstOrDefault(x => x.Id == Usuarios.GetUsuario()).Email;

            return e;
        }

        public static string getEmailUsuarioAS()
        {
            SASEntities db = new SASEntities();
            var e = db.AspNetUsers.FirstOrDefault(x => x.Id == Usuarios.GetUsuario()).Email;

            return e;
        }
    }
}