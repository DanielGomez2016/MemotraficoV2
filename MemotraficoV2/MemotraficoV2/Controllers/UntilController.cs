﻿using MemotraficoV2.Models;
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
using System.Net.Mail;
using System.Collections.Specialized;

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

        public JsonResult CargarDepto(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                List<Departamento> list = db.Departamento.Where(i => i.IdInstitucionFk == id).ToList();

                List<string> rows = new List<string>();
                var rowfirst = "<option>Seleciona el departamento</option>";
                rows.Add(rowfirst);
                foreach (var x in list)
                {
                    var option = "<option value=\"" + x.IdDepartamento + "\">" + x.Nombre + "</option>";
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

        public JsonResult CargarContactoEscuela(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                var contacto = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == id);


                return Json(new
                {
                    tel = contacto.Telefono,
                    email = contacto.Email,
                    director = contacto.Nombre,
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

        [Authorize(Roles = "Admin")]
        public ActionResult ImportarExcel()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
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
                        var clave = ds.Tables[0].Rows[i][1].ToString();
                        if (!db.Escuela.Any(j => j.Clave == clave))
                        {
                            e.Clave = ds.Tables[0].Rows[i][1].ToString();
                            e.Nombre = ds.Tables[0].Rows[i][2].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][5].ToString();
                            e.IdMunicipioFk = Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString());
                            var loc = Localidades.IdLocalidades(ds.Tables[0].Rows[i][13].ToString(), Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString()));

                            if(loc > 0)
                            {
                                e.IdLocalidadFk = loc;
                            }else
                            {
                                e.IdLocalidadFk = Localidades.CrearLocalidad(ds.Tables[0].Rows[i][14].ToString(), Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString()), ds.Tables[0].Rows[i][13].ToString());
                            }
                            //e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][3].ToString();
                            //e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            //e.Geoy = ds.Tables[0].Rows[i][8].ToString();
                            e.Estatus = ds.Tables[0].Rows[i][4].ToString();
                            if (ds.Tables[0].Rows[i][9].ToString() != "")
                            {
                                e.CodigoPostal = Convert.ToInt32(ds.Tables[0].Rows[i][9].ToString());
                            }
                            e.Colonia = ds.Tables[0].Rows[i][10].ToString();
                            e.Marginacion = ds.Tables[0].Rows[i][15].ToString();
                            e.Poblacion = ds.Tables[0].Rows[i][16].ToString();
                            var zona = ds.Tables[0].Rows[i][23].ToString();
                            if (!zona.Contains("NA") && !zona.Contains(" "))
                            {
                                e.Zona = Convert.ToInt32(zona);
                            }
                            var id = e.Crear();

                            Contacto c = new Contacto();
                            c.IdEscuelaFk = id;
                            c.Nombre = ds.Tables[0].Rows[i][28].ToString();
                            //c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][25].ToString();
                            //c.Celular = ds.Tables[0].Rows[i][12].ToString();
                            c.Crear();
                        }
                        else
                        {
                            e = db.Escuela.FirstOrDefault(j => j.Clave == clave);
                            e.Nombre = ds.Tables[0].Rows[i][2].ToString();
                            e.Domicilio = ds.Tables[0].Rows[i][5].ToString();
                            e.IdMunicipioFk = Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString());
                            e.IdLocalidadFk = Localidades.IdLocalidades(ds.Tables[0].Rows[i][13].ToString(), Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString()));
                            //e.IdNivelEducativo = NivelEducativo.IdNivelEdu(ds.Tables[0].Rows[i][5].ToString());
                            e.Turno = ds.Tables[0].Rows[i][3].ToString();
                            //e.Geox = ds.Tables[0].Rows[i][7].ToString();
                            //e.Geoy = ds.Tables[0].Rows[i][8].ToString();
                            e.Estatus = ds.Tables[0].Rows[i][4].ToString();
                            if (ds.Tables[0].Rows[i][9].ToString() != null && ds.Tables[0].Rows[i][9].ToString() != "")
                            {
                                e.CodigoPostal = Convert.ToInt32(ds.Tables[0].Rows[i][9].ToString());
                            }
                            e.Colonia = ds.Tables[0].Rows[i][10].ToString();
                            e.Marginacion = ds.Tables[0].Rows[i][15].ToString();
                            e.Poblacion = ds.Tables[0].Rows[i][16].ToString();
                            var zona = ds.Tables[0].Rows[i][23].ToString();
                            if (!zona.Contains("NA") && !zona.Contains(" "))
                            {
                                e.Zona = Convert.ToInt32(zona);
                            }
                            var id2 = e.Editar();

                            Contacto c = new Contacto();
                            c = db.Contacto.FirstOrDefault(j => j.IdEscuelaFk == id2);
                            c.IdEscuelaFk = id2;
                            c.Nombre = ds.Tables[0].Rows[i][28].ToString();
                            //c.Email = ds.Tables[0].Rows[i][10].ToString();
                            c.Telefono = ds.Tables[0].Rows[i][25].ToString();
                            //c.Celular = ds.Tables[0].Rows[i][12].ToString();
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
            catch(Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Until/ImportarExcel",
                    msj = "Error al hace el import, Se agregaron y/o modificaron " + con + " escuelas"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult ImportExcelML(HttpPostedFileBase file)
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
                        Municipios m = new Municipios();
                        var n = ds.Tables[0].Rows[i][0].ToString();
                        if (!db.Municipios.Any(j => j.Nombre == n))
                        {
                            m.Nombre = ds.Tables[0].Rows[i][0].ToString();
                            var id = m.Crear();

                            Localidades l = new Localidades();
                            l.IdMunicipioFk = id;
                            l.Nombre = ds.Tables[0].Rows[i][2].ToString();
                            l.ClaveLocalidad = ds.Tables[0].Rows[i][1].ToString();
                            l.Latitud = ds.Tables[0].Rows[i][3].ToString();
                            l.Longitud = ds.Tables[0].Rows[i][4].ToString();
                            l.Altitud = Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString());
                            l.Crear();
                        }
                        else
                        {
                            var id = m.get(ds.Tables[0].Rows[i][0].ToString());
                            var loc = ds.Tables[0].Rows[i][1].ToString();
                            Localidades l = new Localidades();

                            if (!db.Localidades.Any(j => j.ClaveLocalidad == loc && j.IdMunicipioFk == id))
                            {
                                l.IdMunicipioFk = id;
                                l.Nombre = ds.Tables[0].Rows[i][2].ToString();
                                l.ClaveLocalidad = ds.Tables[0].Rows[i][1].ToString();
                                l.Latitud = ds.Tables[0].Rows[i][3].ToString();
                                l.Longitud = ds.Tables[0].Rows[i][4].ToString();
                                l.Altitud = Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString());
                                l.Crear();
                            }else
                            {
                                l = db.Localidades.FirstOrDefault(x => x.ClaveLocalidad == loc);
                                l.IdMunicipioFk = id;
                                l.Nombre = ds.Tables[0].Rows[i][2].ToString();
                                l.ClaveLocalidad = ds.Tables[0].Rows[i][1].ToString();
                                l.Latitud = ds.Tables[0].Rows[i][3].ToString();
                                l.Longitud = ds.Tables[0].Rows[i][4].ToString();
                                l.Altitud = Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString());
                                l.Editar();
                            }

                        }
                        con++;
                    }
                    #endregion
                }

                return Json(new
                {
                    result = true,
                    dir = "/Until/ImportarExcel",
                    msj = "Se agregaron y/o modificaron " + con + " municipios y localidades"
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = false,
                    dir = "/Until/ImportarExcel",
                    msj = "Error al hace el import, Se agregaron y/o modificaron " + con + " municipios y localidades"
                });
            }
        }


    }
}