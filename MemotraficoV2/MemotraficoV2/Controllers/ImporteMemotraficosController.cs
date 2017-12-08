using MemotraficoV2.Models;
using MemotraficoV2.Models.Colecciones;
using MemotraficoV2.Models.ConexionesDb;
using MemotraficoV2.Models.Importaciones;
using MemotraficoV2.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    public class ImporteMemotraficosController : Controller
    {
        // GET: ImporteMemotraficos
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetMemotraficos(AnexGRID grid)
        {
            return Json(
               ImportaMemos.AllMemotraficos(grid)
            );
        }

        [HttpPost]
        public JsonResult Importar(string memotrafico)
        {
            var result = false;
            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                //mandamos traer los datos del memotrafico
                var query = db.Memotraficos.FirstOrDefault(x => x.Memotrafico.Contains(memotrafico));

                //mandamos traer los datos del documento del memotrafico
                var queryDoc = db.MemotraficosDoctos.FirstOrDefault(x => x.IdMemotrafico == query.IdMemotrafico);

                //mandamos traer las canalizaciones que se ha hecho del memotrafico
                var queryCan = db.Memotraficos_Deptos.Where(x => x.IdMemotrafico == query.IdMemotrafico).ToList();

                //llenamos las canalizaciones del memotrafico
                List<canalizacionMemo> listcm = new List<canalizacionMemo>();
                foreach (var n in queryCan)
                {
                    //llenamos cada canalizacion
                    canalizacionMemo cm = new canalizacionMemo();
                    cm.idCanalizacion = Convert.ToInt32(n.IdMemotraficoDepto);
                    cm.deptoenvia = n.Departamentos.Departamento;
                    cm.deptorecibe = n.Departamentos.Departamento;
                    cm.fechaInicio = n.FechaInicio;
                    cm.fechaFin = n.FechaFin;
                    cm.comentario = n.Comentario;


                    //nos traemos el documento de la canalizacion
                    var querycandoc = db.Memotraficos_DeptosDoctos.Where(x => x.IdMemotraficoDepto == n.IdMemotraficoDepto).ToList();

                    if (querycandoc.Count() > 0)
                    {
                        cm.documentoCanalizacion = new List<documentoCanalizacion>();
                        foreach (var x in querycandoc)
                        {
                            //llenamos el documento de la canalizacion
                            documentoCanalizacion dc = new documentoCanalizacion();
                            dc.descripcion = x.Descripcion;
                            dc.nombre = x.NombreArchivo;
                            dc.Documento = x.Docto;

                            cm.documentoCanalizacion.Add(dc);
                        }
                    }
                    listcm.Add(cm);
                }

                //llenamos el documento del memotrafico
                DocumentoMemo dm = new DocumentoMemo();
                if (queryDoc != null)
                {
                    dm.descripcion = queryDoc.Descripcion;
                    dm.nombre = queryDoc.NombreArchivo;
                    dm.Documento = queryDoc.Docto;
                }

                //llenamos el memotrafico que se va a importar
                memotraficoImport m = new memotraficoImport();
                m.idmemotrafico = Convert.ToInt32(query.IdMemotrafico);
                m.folio = query.Memotrafico;
                m.fechaEntrega = query.Fecha;
                m.asunto = query.Asunto;
                m.idestatus = Convert.ToInt32(query.IdEstatusMemo);
                m.procedencia = ImportaMemos.Procedencia(query.Procedencias.Procedencia);
                m.idasunto = ImportaMemos.TipoAsunto(query.TipoAsuntos.Asunto);
                m.escuela = ImportaMemos.EsEscuela(query.Beneficiarios.clave, query.Beneficiarios.Beneficiario);
                m.idestatus = Convert.ToInt32(query.IdEstatusMemo);
                m.cerrado = query.Cerrado;
                m.cancelado = query.Cancelado;
                m.fechaCanalizado = query.FechaCanalizada;
                m.fechaCancelado = query.FechaCancelado;
                m.fechaDocumento = query.FechaDoctoOrigen;
                m.tiporespuesta = ImportaMemos.TipoRespuesta(query.Check1, query.Check2, query.Check3, query.Check4, query.Check5, query.Check6, query.Check7);
                m.documentomemo = dm;
                m.canalizaciones = listcm;

                result = ImportaMemos.Importar(m);
            }
            catch (Exception e)
            {

            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult ImportarTodoMemotraficos()
        {
            var result = false;
            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                SASEntities dbb = new SASEntities();

                var querytotal = db.Memotraficos.ToList();

                foreach (var i in querytotal)
                {
                    var Existe = dbb.Solicitudes.Any(x => x.Folio.Contains(i.Clave));

                    if (!Existe)
                    {
                        //mandamos traer los datos del memotrafico
                        var query = db.Memotraficos.FirstOrDefault(x => x.Memotrafico.Contains(i.Clave));

                        //mandamos traer los datos del documento del memotrafico
                        var queryDoc = db.MemotraficosDoctos.FirstOrDefault(x => x.IdMemotrafico == query.IdMemotrafico);

                        //mandamos traer las canalizaciones que se ha hecho del memotrafico
                        var queryCan = db.Memotraficos_Deptos.Where(x => x.IdMemotrafico == query.IdMemotrafico).ToList();

                        //llenamos las canalizaciones del memotrafico
                        List<canalizacionMemo> listcm = new List<canalizacionMemo>();
                        foreach (var n in queryCan)
                        {
                            //llenamos cada canalizacion
                            canalizacionMemo cm = new canalizacionMemo();
                            cm.idCanalizacion = Convert.ToInt32(n.IdMemotraficoDepto);
                            cm.deptoenvia = n.Departamentos.Departamento;
                            cm.deptorecibe = n.Departamentos.Departamento;
                            cm.fechaInicio = n.FechaInicio;
                            cm.fechaFin = n.FechaFin;
                            cm.comentario = n.Comentario;


                            //nos traemos el documento de la canalizacion
                            var querycandoc = db.Memotraficos_DeptosDoctos
                                                .Where(x => x.IdMemotraficoDepto == n.IdMemotraficoDepto).ToList();

                            if (querycandoc.Count() > 0)
                            {
                                cm.documentoCanalizacion = new List<documentoCanalizacion>();
                                foreach (var x in querycandoc)
                                {
                                    //llenamos el documento de la canalizacion
                                    documentoCanalizacion dc = new documentoCanalizacion();
                                    dc.descripcion = x.Descripcion;
                                    dc.nombre = x.NombreArchivo;
                                    dc.Documento = x.Docto;

                                    cm.documentoCanalizacion.Add(dc);
                                }
                            }
                            listcm.Add(cm);
                        }

                        //llenamos el documento del memotrafico
                        DocumentoMemo dm = new DocumentoMemo();
                        if (queryDoc != null)
                        {
                            dm.descripcion = queryDoc.Descripcion;
                            dm.nombre = queryDoc.NombreArchivo;
                            dm.Documento = queryDoc.Docto;
                        }

                        //llenamos el memotrafico que se va a importar
                        memotraficoImport m = new memotraficoImport();
                        m.idmemotrafico = Convert.ToInt32(query.IdMemotrafico);
                        m.folio = query.Memotrafico;
                        m.fechaEntrega = query.Fecha;
                        m.asunto = query.Asunto;
                        m.idestatus = Convert.ToInt32(query.IdEstatusMemo);
                        m.procedencia = ImportaMemos.Procedencia(query.Procedencias.Procedencia);
                        m.idasunto = ImportaMemos.TipoAsunto(query.TipoAsuntos.Asunto);
                        m.escuela = ImportaMemos.EsEscuela(query.Beneficiarios.clave, query.Beneficiarios.Beneficiario);
                        m.idestatus = Convert.ToInt32(query.IdEstatusMemo);
                        m.cerrado = query.Cerrado;
                        m.cancelado = query.Cancelado;
                        m.fechaCanalizado = query.FechaCanalizada;
                        m.fechaCancelado = query.FechaCancelado;
                        m.fechaDocumento = query.FechaDoctoOrigen;
                        m.tiporespuesta = ImportaMemos.TipoRespuesta(query.Check1, query.Check2, query.Check3, query.Check4, query.Check5, query.Check6, query.Check7);
                        m.documentomemo = dm;
                        m.canalizaciones = listcm;

                        result = ImportaMemos.Importar(m);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetBeneficiarios(AnexGRID grid)
        {
            return Json(
               ImportaMemos.AllBeneficiarios(grid)
            );
        }

        [HttpPost]
        public JsonResult ImportarTodoBeneficiario()
        {
            var cont = 0;
            var cont2 = 0;
            var result = false;
            var actualizado = false;
            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                //mandamos traer los datos del beneficiario
                var query = db.Beneficiarios.ToList();

                foreach (var k in query)
                {
                    //localidades y municipios
                    var municipio = db.Municipios.FirstOrDefault(x => x.IdMunicipio == k.IdMunicipio).Municipio;
                    var localidad = db.Localidades.Any(x => x.IdLocalidad == k.IdLocalidad && x.IdMunicipio == k.IdMunicipio) != false ?
                                    db.Localidades.FirstOrDefault(x => x.IdLocalidad == k.IdLocalidad && x.IdMunicipio == k.IdMunicipio).Localidad : "";

                    var nivel = db.NivelesEducativos.FirstOrDefault(x => x.IdNivelEducativo == k.IdNivelEducativo).NivelEducativo;

                    //mandamos traer los datos del contacto del beneficiario
                    var querytel = db.TelefonosBeneficiarios.FirstOrDefault(x => x.IdBeneficiario == k.IdBeneficiario);

                    //mandamos traer el tipo de telefono del bebeficiario
                    var querytt = "";
                    if (querytel != null)
                    {
                        querytt = db.TiposTelefono.FirstOrDefault(x => x.IdTipoTelefono == querytel.IdTipoTelefono).TipoTelefono;
                    }

                    beneficiariosImport b = new beneficiariosImport();

                    //verificamos que el registro no exista en escuelas ni beneficiarios
                    SASEntities dbb = new SASEntities();
                    if (!dbb.Beneficiario.Any(x => x.Nombre.Contains(k.Beneficiario)) && !dbb.Escuela.Any(x => x.Clave == k.clave))
                    {
                        b.EsEscuela = false;
                        b.beneficiario = k.Beneficiario;
                        b.clave = k.clave;
                        b.domicilio = k.Domicilio;
                        b.localidad = ImportaMemos.IdLocalidad(localidad);
                        b.municipio = ImportaMemos.IdMunicipio(municipio);
                        b.director = k.Director;
                        b.idniveleducativo = ImportaMemos.NivelEducativo(nivel);
                        b.telefono = querytel != null ? querytel.Telefono : "";
                        b.tipotelefono = querytt;
                    }

                    if (dbb.Escuela.Any(x => x.Clave == k.clave))
                    {
                        b.EsEscuela = true;
                        b.clave = k.clave;
                        b.idniveleducativo = ImportaMemos.NivelEducativo(nivel);
                        actualizado = ImportaMemos.ImportarBeneficiario(b);
                        if (actualizado)
                        {
                            cont2++;
                        }
                    }

                    if (b.EsEscuela == false && b.beneficiario != null)
                    {
                        result = ImportaMemos.ImportarBeneficiario(b);
                        if (result)
                        {
                            cont++;
                        }
                    }
                } 
            }
            catch (Exception e)
            {

            }

            return Json(new {
                result = result,
                contador = cont,
                actualizado = actualizado,
                contadora = cont2
            });
        }

        [HttpPost]
        public JsonResult ImportarBeneficiario(int id)
        {
            var cont = 0;
            var cont2 = 0;
            var result = false;
            var actualizado = false;
            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                //mandamos traer los datos del beneficiario
                var query = db.Beneficiarios.FirstOrDefault(x => x.IdBeneficiario == id);

                //localidades y municipios
                var municipio = db.Municipios.FirstOrDefault(x => x.IdMunicipio == query.IdMunicipio).Municipio;
                var localidad = db.Localidades.Any(x => x.IdLocalidad == query.IdLocalidad && x.IdMunicipio == query.IdMunicipio) != false ?
                                db.Localidades.FirstOrDefault(x => x.IdLocalidad == query.IdLocalidad && x.IdMunicipio == query.IdMunicipio).Localidad : "";

                var nivel = db.NivelesEducativos.FirstOrDefault(x => x.IdNivelEducativo == query.IdNivelEducativo).NivelEducativo;

                //mandamos traer los datos del contacto del beneficiario
                var querytel = db.TelefonosBeneficiarios.FirstOrDefault(x => x.IdBeneficiario == query.IdBeneficiario);

                //mandamos traer el tipo de telefono del bebeficiario
                var querytt = "";
                if (querytel != null)
                {
                    querytt = db.TiposTelefono.FirstOrDefault(x => x.IdTipoTelefono == querytel.IdTipoTelefono).TipoTelefono;
                }

                beneficiariosImport b = new beneficiariosImport();

                //verificamos que el registro no exista en escuelas ni beneficiarios
                SASEntities dbb = new SASEntities();
                if (!dbb.Beneficiario.Any(x => x.Nombre.Contains(query.Beneficiario)) && !dbb.Escuela.Any(x => x.Clave == query.clave))
                {
                    b.EsEscuela = false;
                    b.beneficiario = query.Beneficiario;
                    b.clave = query.clave;
                    b.domicilio = query.Domicilio;
                    b.localidad = ImportaMemos.IdLocalidad(localidad);
                    b.municipio = ImportaMemos.IdMunicipio(municipio);
                    b.director = query.Director;
                    b.idniveleducativo = ImportaMemos.NivelEducativo(nivel);
                    b.telefono = querytel != null ? querytel.Telefono : "";
                    b.tipotelefono = querytt;
                }

                if (dbb.Escuela.Any(x => x.Clave == query.clave))
                {
                    b.EsEscuela = true;
                    b.clave = query.clave;
                    b.idniveleducativo = ImportaMemos.NivelEducativo(nivel);
                    actualizado = ImportaMemos.ImportarBeneficiario(b);
                    if (actualizado)
                    {
                        cont2++;
                    }
                }

                if (b.EsEscuela == false && b.beneficiario != null)
                {
                    result = ImportaMemos.ImportarBeneficiario(b);
                    if (result)
                    {
                        cont++;
                    }
                }
            
            }
            catch (Exception e)
            {

            }

            return Json(new {
                result = result,
                contador = cont,
                actualizado = actualizado,
                contadora = cont2
            });
        }


        [HttpPost]
        public JsonResult GetProcedencia(AnexGRID grid)
        {
            return Json(
               ImportaMemos.AllProcedencia(grid)
            );
        }

        [HttpPost]
        public JsonResult ImportarTodaProcedencia()
        {
            var cont = 0;
            var result = false;

            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                //mandamos traer los datos del beneficiario
                var query = db.Procedencias.ToList();

                foreach (var k in query)
                {
                    //localidades y municipios
                    var municipio = db.Municipios.FirstOrDefault(x => x.IdMunicipio == k.IdMunicipio).Municipio;
                    var localidad = db.Localidades.Any(x => x.IdLocalidad == k.IdLocalidad && x.IdMunicipio == k.IdMunicipio) != false ?
                                    db.Localidades.FirstOrDefault(x => x.IdLocalidad == k.IdLocalidad && x.IdMunicipio == k.IdMunicipio).Localidad : "";

                    procedenciaImport p = new procedenciaImport();

                    //verificamos que el registro no exista en procedencias
                    SASEntities dbb = new SASEntities();

                    if (!dbb.Procedencia.Any(x => x.Procedencia1.Contains(k.Procedencia)))
                    {
                        p.procedencia = k.Procedencia;
                        p.domicilio = k.Domicilio;
                        p.municipio = ImportaMemos.IdMunicipio(municipio);
                        p.localidad = ImportaMemos.IdLocalidad(localidad);
                        p.contacto = k.Contacto;
                        p.tipoprocedencia = k.TipoProcedencias.TipoProcedencia.ToString();
                        
                        result = ImportaMemos.ImportarProcedencia(p);

                        if (result) { cont++; }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return Json(new
            {
                result = result,
                contador = cont,
            });
        }

        [HttpPost]
        public JsonResult ImportarProcedencia(int id)
        {
            var result = false;
        
            try
            {
                MemotraficosEntities db = new MemotraficosEntities();
                //mandamos traer los datos de la procedencia
                var query = db.Procedencias.FirstOrDefault(x => x.IdProcedencia == id);

                //localidades y municipios
                var municipio = db.Municipios.FirstOrDefault(x => x.IdMunicipio == query.IdMunicipio).Municipio;
                var localidad = db.Localidades.Any(x => x.IdLocalidad == query.IdLocalidad && x.IdMunicipio == query.IdMunicipio) != false ?
                                db.Localidades.FirstOrDefault(x => x.IdLocalidad == query.IdLocalidad && x.IdMunicipio == query.IdMunicipio).Localidad : "";

                procedenciaImport p = new procedenciaImport();

                //verificamos que el registro no exista en procedencias
                SASEntities dbb = new SASEntities();

                if(!dbb.Procedencia.Any(x => x.Procedencia1.Contains(query.Procedencia)))
                {
                    p.procedencia = query.Procedencia;
                    p.domicilio = query.Domicilio;
                    p.municipio = ImportaMemos.IdMunicipio(municipio);
                    p.localidad = ImportaMemos.IdLocalidad(localidad);
                    p.contacto = query.Contacto;
                    p.tipoprocedencia = query.TipoProcedencias.TipoProcedencia.ToString();

                    result = ImportaMemos.ImportarProcedencia(p);
                }

            }
            catch (Exception e)
            {

            }

            return Json(result);
        }
    }
    }
