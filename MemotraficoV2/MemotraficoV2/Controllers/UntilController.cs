using MemotraficoV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    public class UntilController : Controller
    {
        public JsonResult CargarLocalidad(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                List<Localidades> list = db.Localidades.Where(i => i.IdMunicipioFk == id).ToList();

                List<string> rows = new List<string>();
                var rowfirst = "<option>Seleciona alguna localidad</option>";
                rows.Add(rowfirst);
                foreach (var x in list)
                {
                    var option = "<option value=\"" + x.IdLocalidad + "\">" + x.Nombre + "</option>";
                    rows.Add(option);
                }

                return Json(new
                {
                    total = rows.Count(),
                    datos = rows.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = true
                });
            }
        }

        public JsonResult CargarProcedencia(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                List<Procedencia> list = db.Procedencia.Where(i => i.IdTipoProcedenciaFk == id).ToList();

                List<string> rows = new List<string>();
                var rowfirst = "<option>Seleciona la procedencia</option>";
                rows.Add(rowfirst);
                foreach (var x in list)
                {
                    var option = "<option value=\"" + x.IdProcedencia + "\">" + x.Procedencia1 + "</option>";
                    rows.Add(option);
                }

                return Json(new
                {
                    total = rows.Count(),
                    datos = rows.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = true
                });
            }
        }

        public JsonResult CargarUsuarios(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                List<AspNetUsers> list = db.AspNetUsers.Where(i => i.IdDepartamento == id).ToList();

                List<string> rows = new List<string>();
                var rowfirst = "<option>Seleciona algun Usuario</option>";
                rows.Add(rowfirst);
                foreach (var x in list)
                {
                    var nom = x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno;
                    var option = "<option value=\"" + x.Id + "\">" + nom + "</option>";
                    rows.Add(option);
                }

                return Json(new
                {
                    total = rows.Count(),
                    datos = rows.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = true
                });
            }
        }
    }
}