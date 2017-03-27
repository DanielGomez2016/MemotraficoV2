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
    }
}