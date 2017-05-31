using MemotraficoV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;

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

        public ActionResult ImportarExcel()
        {
            return View();
        }

        public JsonResult ImportExcelEscuelas(HttpPostedFileBase file)
        {
            var con = 0;
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //localizacion del archio donde se guardara
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                        //guardar el archivo
                        Request.Files["file"].SaveAs(fileLocation);

                        #region cadena de conexion del excel
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        #endregion

                        #region crear una conexion con el excel, busqueda de los registros a importar
                        //crear la conexion del excel y abrir conexion
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        #endregion
                    }

                    #region guardar o modificar registros de escuelas con su contacto
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        SASEntities db = new SASEntities();
                        Escuela e = new Escuela();
                        var clave = ds.Tables[0].Rows[i][0].ToString();
                        if (!db.Escuela.Any(j => j.Clave == clave))
                        {
                            e.Clave = ds.Tables[0].Rows[i][0].ToString();
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();

                            var id = e.Crear();

                            Contacto c = new Contacto();
                            c.IdEscuelaFk = id;
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Crear();
                        }
                        else
                        {
                            e = db.Escuela.FirstOrDefault(j => j.Clave == clave);
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();
                            var id2 = e.Editar();

                            Contacto c = new Contacto();
                            c = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == id2);
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Editar();

                        }
                        con++;
                    }
                    #endregion
                }

                return Json(new
                {
                    result = true,
                    dir = "/Until/ImportarExcel",
                    msj = "Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Until/ImportarExcel",
                    msj = "Error al hace el import, Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
        }

        public JsonResult ImportExcelMunicipios(HttpPostedFileBase file)
        {
            var con = 0;
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //localizacion del archio donde se guardara
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                        //guardar el archivo
                        Request.Files["file"].SaveAs(fileLocation);

                        #region cadena de conexion del excel
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        #endregion

                        #region crear una conexion con el excel, busqueda de los registros a importar
                        //crear la conexion del excel y abrir conexion
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        #endregion
                    }

                    #region guardar o modificar registros de escuelas con su contacto
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        SASEntities db = new SASEntities();
                        Escuela e = new Escuela();
                        var clave = ds.Tables[0].Rows[i][0].ToString();
                        if (!db.Escuela.Any(j => j.Clave == clave))
                        {
                            e.Clave = ds.Tables[0].Rows[i][0].ToString();
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();

                            var id = e.Crear();

                            Contacto c = new Contacto();
                            c.IdEscuelaFk = id;
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Crear();
                        }
                        else
                        {
                            e = db.Escuela.FirstOrDefault(j => j.Clave == clave);
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();
                            var id2 = e.Editar();

                            Contacto c = new Contacto();
                            c = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == id2);
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Editar();

                        }
                        con++;
                    }
                    #endregion
                }

                return Json(new
                {
                    result = true,
                    dir = "/Until/ImportarExcel",
                    msj = "Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Until/ImportarExcel",
                    msj = "Error al hace el import, Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
        }

        public JsonResult ImportExcelLocalidades(HttpPostedFileBase file)
        {
            var con = 0;
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //localizacion del archio donde se guardara
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                        //guardar el archivo
                        Request.Files["file"].SaveAs(fileLocation);

                        #region cadena de conexion del excel
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        #endregion

                        #region crear una conexion con el excel, busqueda de los registros a importar
                        //crear la conexion del excel y abrir conexion
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        #endregion
                    }

                    #region guardar o modificar registros de escuelas con su contacto
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        SASEntities db = new SASEntities();
                        Escuela e = new Escuela();
                        var clave = ds.Tables[0].Rows[i][0].ToString();
                        if (!db.Escuela.Any(j => j.Clave == clave))
                        {
                            e.Clave = ds.Tables[0].Rows[i][0].ToString();
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();

                            var id = e.Crear();

                            Contacto c = new Contacto();
                            c.IdEscuelaFk = id;
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Crear();
                        }
                        else
                        {
                            e = db.Escuela.FirstOrDefault(j => j.Clave == clave);
                            e.Nombre = ds.Tables[0].Rows[i][1].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][2].ToString();
                            e.IdMunicipioFk = Municipios.IdMunicipios(ds.Tables[0].Rows[i][3].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][4].ToString());
                            e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][6].ToString();
                            e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            e.Geoy = ds.Tables[0].Rows[i][8].ToString();
                            var id2 = e.Editar();

                            Contacto c = new Contacto();
                            c = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == id2);
                            c.Nombre = ds.Tables[0].Rows[i][9].ToString();
                            c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][11].ToString();
                            c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Editar();

                        }
                        con++;
                    }
                    #endregion
                }

                return Json(new
                {
                    result = true,
                    dir = "/Until/ImportarExcel",
                    msj = "Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
            catch
            {
                return Json(new
                {
                    result = false,
                    dir = "/Until/ImportarExcel",
                    msj = "Error al hace el import, Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
        }

    }
}