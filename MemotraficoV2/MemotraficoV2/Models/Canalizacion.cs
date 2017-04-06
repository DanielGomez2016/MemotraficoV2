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

        public static int Canalizar(int s)
        {
            Canalizacion c = new Canalizacion();

            c.IdInstitucionFk = Usuarios.GetInstitucion();
            c.Validacion = Convert.ToBoolean(ListaValidaciones.NO_VALIDACION);
            c.IdSolicitudFk = s;

            int v2 = c.Crear();
            DetalleCanalizacion dc = new DetalleCanalizacion();
            dc.IdCanalizarFk = v2;
            dc.FechaCanalizar = DateTime.Now;
            dc.Comentario = ListaComentarios.INICIADA;
            dc.IdUsuarioFk = Usuarios.GetUsuario();
            dc.Departamento = Usuarios.GetDepto();
            dc.Instituto = Usuarios.GetInstitucion();
            var v3 = dc.Crear();

            return v3;
        }
    }
}