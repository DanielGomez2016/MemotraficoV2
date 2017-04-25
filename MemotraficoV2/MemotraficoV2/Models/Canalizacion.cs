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
                if(usuarioasigna == "")
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

            return canalizaDet;
        }

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
        }

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
            dc.IdUsuarioFk = Usuarios.RolAlto(Usuarios.Roles(), Usuarios.GetInstitucion());
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = estatus;
            dc.Crear();
        }

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
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            dc.IdEstatusFk = ListaEstatus.CANALIZADO;
            dc.UsuarioAtiende = Usuarios.GetUsuario();
            return dc.Crear();
        }

    }
}