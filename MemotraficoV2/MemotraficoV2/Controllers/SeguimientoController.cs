using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemotraficoV2.Models;
using MemotraficoV2.ViewModels;
using MemotraficoV2.Models.Colecciones;
using System.Collections;
using MemotraficoV2.Filters;

namespace MemotraficoV2.Controllers
{
    public class SeguimientoController : Controller
    {
        // GET: Seguimiento
        public ActionResult Index(string folio)
        {
            SASEntities db = new SASEntities();

            SolicitudDetalle sd = new SolicitudDetalle();
            sd.Detalles = new List<Detalle>();

            var s = db.Solicitudes.FirstOrDefault(x => x.Folio == folio);

            int idcanalizacion = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == s.IdSolicitud).IdCanalizacion;
            var ix = 0;
            List<DetalleCanalizacion> dc = db.DetalleCanalizacion
                .Where(j => j.IdCanalizarFk == idcanalizacion)
                .OrderByDescending(i => i.IdDetalleCanalizar).ToList();
            ix = dc.Count();
            foreach (var x in dc)
            {
                Detalle d = new Detalle();

                d.FechaCanalizado = x.FechaCanalizar.Value.ToString("dd/MM/yyyy") + " " + x.FechaCanalizar.Value.ToString("HH:mm:ss");
                d.Comentario = x.Comentario;
                d.usuario = Usuarios.GetUsuarioId(x.IdUsuarioFk);
                d.departamento = Departamento.getNombre(x.Departamento);
                d.usuarioatiende = Usuarios.GetUsuarioId(x.UsuarioAtiende);
                d.estatus = x.Estatus.Estatus1;
                d.numregistro = ix;

                switch (x.Estatus.IdEstatus)
                {
                    case ListaEstatus.INICIADO:
                        d.colorreg = "info";
                        break;
                    case ListaEstatus.CANALIZADO:
                        d.colorreg = "success";
                        break;
                    case ListaEstatus.CANCELADO:
                        d.colorreg = "warning";
                        break;
                    case ListaEstatus.CERRADO:
                        d.colorreg = "danger";
                        break;
                    case ListaEstatus.ATENDIDA:
                        d.colorreg = "success";
                        break;
                    default:
                        d.colorreg = "";
                        break;
                }

                d.docs = db.Documentos.Where(i => i.IdDetalleCanalizarFk == x.IdDetalleCanalizar).ToList();

                sd.Detalles.Add(d);
                ix--;
            }
            sd.solicitud = db.Solicitudes.FirstOrDefault(i => i.IdSolicitud == s.IdSolicitud);

            var ins = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == s.IdSolicitud).IdInstitucionFk;

            sd.institucion = Institucion.getinstitucionName(ins);
            sd.Detalles.OrderBy(i => i.numregistro);
            return View(sd);
        }

        public ActionResult DownFile(int idfile)
        {
            SASEntities db = new SASEntities();
            var f = db.Documentos.FirstOrDefault(i => i.IdDocumento == idfile);
            if (f != null)
            {
                string type = string.Empty;
                string ext = string.Empty;

                switch (f.Tipo)
                {
                    case "application/pdf":
                        type = "application/pdf";
                        ext = ".pdf";
                        break;
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        type = "aapplication/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        ext = ".docx";
                        break;
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        ext = ".xlsx";
                        break;
                    case "application/vnd.ms-excel":
                        type = "application/vnd.ms-excel";
                        ext = ".xls";
                        break;
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        type = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        ext = ".pptx";
                        break;
                    case "application/vnd.ms-powerpoint":
                        type = "application/vnd.ms-powerpoint";
                        ext = ".ppt";
                        break;
                    case "image/jpeg":
                        type = "image/jpeg";
                        ext = ".jpge";
                        break;
                }
                var file = File(f.Documento, type);
                file.FileDownloadName = f.Nombre + ext;
                return file;
            }
            else
            {
                return null;
            }
        }
    }   
}
