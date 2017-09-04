using MemotraficoV2.Filters;
using MemotraficoV2.Models;
using MemotraficoV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemotraficoV2.Controllers
{
    [Authorize, Acceso]
    public class ValidacionesController : Controller
    {
        // GET: Validaciones
        public ActionResult Index()
        {

            SASEntities db = new SASEntities();
            List<EscuelaValidacion> Escuelas = db.Escuela
                                                 .Select(x => new EscuelaValidacion
                                                 {
                                                     Id = x.IdEscuela,
                                                     Nombre = x.Nombre,
                                                     Clave = x.Clave,
                                                     TotalValidaciones = x.Validacion
                                                                          .Where(z => z.IdEscuelaFk == x.IdEscuela)
                                                                          .Count(),
                                                     UltimaValidacion = db.Validacion
                                                                          .FirstOrDefault(z =>
                                                                                          z.IdEscuelaFk == x.IdEscuela &&
                                                                                          z.Historial == false) != null ? db.Validacion
                                                                          .FirstOrDefault(z =>
                                                                                          z.IdEscuelaFk == x.IdEscuela &&
                                                                                          z.Historial == false)
                                                                          .FechaValidacion.ToString() : ""
                                                 }).ToList();
            return View(Escuelas);
        }

        public ActionResult ValidacionEscolar(int escuela, string solicitud, int crear)
        {
            SASEntities db = new SASEntities();
            Validaciones vr = new Validaciones();

            vr.Solicitudes = solicitud != "" ? db.Solicitudes.FirstOrDefault(i => i.Folio == solicitud) : null;

            vr.FolioSolicitud = solicitud;
            Contacto c = db.Contacto.FirstOrDefault(i => i.IdEscuelaFk == escuela);

            vr.Escuela = db.Escuela.FirstOrDefault(i => i.IdEscuela == escuela);
            vr.Escuela.Celular = c.Celular;
            vr.Escuela.NombreDirector = c.Nombre;
            vr.Escuela.Telefono = c.Telefono;
            vr.Escuela.EmailDirector = c.Email;

            //se crear registro de validacion si ya se tiene se regresa el registro
            if(crear > 0)
            {
                vr.validacion = Validacion.EditarRegistro(escuela);
            }
            else
            {
                var can = db.Validacion
                            .OrderByDescending(x => x.FechaValidacion)
                            .Where(x => x.IdEscuelaFk == escuela).Any(x => x.Historial == false);
                if (!can)
                {
                    vr.validacion = Validacion.NuevoRegistro(escuela);
                }else
                {
                    vr.validacion = Validacion.EditarRegistro(escuela);
                }   
            }

            //llenar los campos de aulas,laboratorios,talleres y anexos
            vr.Aulas = EspacioEducativoDet.ContarAulas(vr.validacion == null ? 0 : vr.validacion.IdValidar);
            vr.Laboratorios = EspacioEducativoDet.ContarLaboratorios(vr.validacion == null ? 0 : vr.validacion.IdValidar);
            vr.Talleres = EspacioEducativoDet.ContarTalleres(vr.validacion == null ? 0 : vr.validacion.IdValidar);
            vr.Anexos = EspacioEducativoDet.ContarAnexos(vr.validacion == null ? 0 : vr.validacion.IdValidar);

            vr.Matricula = Matricula.ObtenerRegistro(vr.validacion.IdValidar);
            vr.Entorno = Entorno.ObtenerRegistro(vr.validacion.IdValidar);
            vr.Croquis = Croquis.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ServicioMunicipal = ServicioMunicipal.ObtenerRegistro(vr.validacion.IdValidar);
            vr.AlmacenamientoDren = AlmacenamientoDren.ObtenerRegistro(vr.validacion.IdValidar);
            vr.EnergiaElectrica = EnergiaElectrica.ObtenerRegistro(vr.validacion.IdValidar);
            vr.EspacioMultiple = EspacioMultiple.ObtenerRegistro(vr.validacion.IdValidar);
            vr.EspacioEducativo = EspacioEducativo.ObtenerRegistro(vr.validacion.IdValidar);
            vr.EspacioEducativoDet = EspacioEducativo.ObtenerRegistros(vr.EspacioEducativo.IdEspacioEducativo);

            vr.ComponenteI = ComponenteI.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteII = ComponenteII.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteIII = ComponenteIII.ObtenerRegistros(vr.validacion.IdValidar);
            vr.ComponenteIV = ComponenteIV.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteV = ComponenteV.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteVI = ComponenteVI.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteVII = ComponenteVII.ObtenerRegistro(vr.validacion.IdValidar);
            vr.ComponenteVIII = ComponenteVIII.ObtenerRegistro(vr.validacion.IdValidar);

            return View(vr);
        }

        [HttpPost]
        public JsonResult Validaciones(Validaciones vr)
        {
            try
            {
                SASEntities db = new SASEntities();

                #region solictud escuela canalizacion
                Solicitudes s = vr.Solicitudes;
                Escuela e = vr.Escuela;

                if(s != null)
                {
                    //editamos los campos de fecha validacion y programa, en el registro de la solicitud
                    s.Editar();

                    //cambiamos la validacion a true lo cual nos indica que la escuela ya tiene una validacion hecha
                    Canalizacion c = db.Canalizacion.FirstOrDefault(i => i.IdSolicitudFk == s.IdSolicitud);
                    c.Validacion = true;
                    c.EditarValidacion();
                }
                if (e != null) {
                    //editamos los campos de geolocalizacion de la escuela
                    e.Editar();
                }
                #endregion

                #region validaciones
                if (vr.validacion.IdValidar > 0)
                {
                    #region Matricula
                    //iniciamos guardando cada modulo de validaciones
                    //primer modulo matricula, espacios educativos
                    Matricula m = vr.Matricula;
                    m.IdValidarFk = vr.validacion.IdValidar;
                    if(vr.Matricula.IdMatricula > 0)
                    {
                        m.Editar();
                    }
                    #endregion

                    #region Entorno
                    //segundo modulo emplazamiento y entorno
                    Entorno en = vr.Entorno;
                    en.IdValidarFk = vr.validacion.IdValidar;
                    en.Infraestructura = vr.EntornoInfraestructura;
                    en.Terreno = vr.EntornoTerreno;

                    if (vr.EntornoAmenaza != null)
                    {
                        foreach (var i in vr.EntornoAmenaza)
                        {
                            switch (i)
                            {
                                case "RioArroyo":
                                    en.Rio_Arrollo = true;
                                    break;
                                case "Gasolinera":
                                    en.Gasolinera = true;
                                    break;
                                case "DerechoVia":
                                    en.DerechoVia = true;
                                    break;
                                case "AmenazaVial":
                                    en.AmenazaVial = true;
                                    break;
                                case "Comercio":
                                    en.Comercio = true;
                                    break;
                            }
                        }
                    }

                    if (en.IdEntorno > 0)
                    {
                        if (en.Rio_Arrollo != true) { en.Rio_Arrollo = false; en.Rio_ArrolloDistancia = ""; }  
                        if (en.Gasolinera != true) { en.Gasolinera = false; en.GasolineraDistancia = ""; }
                        if (en.DerechoVia != true) { en.DerechoVia = false; en.DerechoViaDistancia = ""; }
                        if (en.AmenazaVial != true) { en.AmenazaVial = false; en.AmenazaVialDistancia = ""; }
                        if (en.Comercio != true) { en.Comercio = false; en.ComercioDistancia = ""; }
                        en.Editar();
                    }
                    #endregion

                    #region Croquis
                    //tercer modulo croquis tipo de predio y suelo
                    Croquis c = vr.Croquis;
                    c.IdValidarFk = vr.validacion.IdValidar;
                    c.Predio = vr.CroquisPredio == 1 ? true : false;
                    c.TipoSuelo = vr.CroquisTipoSuelo;

                    if (c.IdCroquis > 0)
                    {
                        c.Editar();
                    }
                    #endregion

                    #region Servicio municipal
                    //cuarto modulo servicios municipales
                    ServicioMunicipal sm = vr.ServicioMunicipal;
                    sm.IdValidarFk = vr.validacion.IdValidar;

                    if (vr.SMRed != null)
                    {
                        foreach (var i in vr.SMRed)
                        {
                            switch (i)
                            {
                                case "RedAgua":
                                    sm.RedAgua = true;
                                    break;
                                case "RedDrenaje":
                                    sm.RedDrenaje = true;
                                    break;
                                case "RedEnergia":
                                    sm.RedEnergia = true;
                                    break;
                                case "RedAlumbrado":
                                    sm.RedAlumbrado = true;
                                    break;
                            }
                        }
                    }

                    if (vr.SMVialidades != null)
                    {
                        foreach (var i in vr.SMVialidades)
                        {
                            switch (i)
                            {
                                case "Primaria":
                                    sm.VialidadPrimaria = true;
                                    sm.VialidadSecundaria = false;
                                    sm.VialidadTerciaria = false;
                                    break;
                                case "Secundaria":
                                    sm.VialidadPrimaria = false;
                                    sm.VialidadSecundaria = true;
                                    sm.VialidadTerciaria = false;
                                    break;
                                case "Terciaria":
                                    sm.VialidadPrimaria = false;
                                    sm.VialidadSecundaria = false;
                                    sm.VialidadTerciaria = true;
                                    break;
                            }
                        }
                    }

                    if (en.IdEntorno > 0)
                    {
                        if (sm.RedAgua != true) { sm.RedAgua = false; sm.RedAguaDistancia = ""; }
                        if (sm.RedDrenaje != true) { sm.RedDrenaje = false; sm.RedDrenajeDistancia = ""; }
                        if (sm.RedEnergia != true) { sm.RedEnergia = false; sm.RedEnergiaDistancia = ""; }
                        if (sm.RedAlumbrado != true) { sm.RedAlumbrado = false; sm.RedAlumbradoDistancia = ""; }
                        if (sm.VialidadPrimaria != true) sm.VialidadPrimaria = false;
                        if (sm.VialidadSecundaria != true) sm.VialidadSecundaria = false;
                        if (sm.VialidadTerciaria != true) sm.VialidadTerciaria = false;
                        sm.Editar();
                    }
                    #endregion

                    #region almacenamiento
                    //quinto modulo almacenamiento y drenaje sanitario
                    AlmacenamientoDren ad = vr.AlmacenamientoDren;
                    ad.IdValidarFk = vr.validacion.IdValidar;
                    ad.EstadoGeneral = vr.ADEdoGeneral;

                    if (vr.AlmanceDren != null)
                    {
                        foreach (var i in vr.AlmanceDren)
                        {
                            switch (i)
                            {
                                case "Agua_Potable":
                                    ad.Redagua = true;
                                    ad.RedaguaEstado = vr.ADAguaPotable;
                                    break;
                                case "Pipas":
                                    ad.Pipa = true;
                                    ad.PipaEstado = vr.ADPipas;
                                    break;
                                case "Cisterna":
                                    ad.Cisterna = true;
                                    ad.CisternaEstado = vr.ADCisterna;
                                    break;
                                case "Tinaco":
                                    ad.Tinaco = true;
                                    ad.TinacoEstado = vr.ADTinaco;
                                    break;
                                case "ColectorM":
                                    ad.ColectorMunicipal = true;
                                    ad.ColectorMunicipalEstado = vr.ADColectorM;
                                    break;
                                case "FosaSeptica":
                                    ad.FosaSeptica = true;
                                    ad.FosaSepticaEstado = vr.ADFosaSeptica;
                                    break;
                            }
                        }
                    }
                    var contador = 0;
                    if (ad.IdAlmacenDrenaje > 0)
                    {
                        if (ad.Redagua != true) { ad.Redagua = false; ad.RedaguaEstado = null; contador += 1; }
                        if (ad.Pipa != true) { ad.Pipa = false; ad.PipaEstado = null; contador += 1; }
                        if (ad.Cisterna != true) { ad.Cisterna = false; ad.CisternaEstado = null; contador += 1; }
                        if (ad.Tinaco != true) { ad.Tinaco = false; ad.TinacoEstado = null; contador += 1; }
                        if (ad.ColectorMunicipal != true) { ad.ColectorMunicipal = false; ad.ColectorMunicipalEstado = null; contador += 1; }
                        if (ad.FosaSeptica != true) { ad.FosaSeptica = false; ad.FosaSepticaEstado = null; contador += 1; }
                        if (contador == 6) { ad.EstadoGeneral = null; }
                        ad.Editar();
                    }
                    #endregion

                    #region Energia electrica
                    //sexto modulo de energia electrica

                    EnergiaElectrica ee = vr.EnergiaElectrica;
                    ee.IdValidarFk = vr.validacion.IdValidar;
                    ee.EstadoGeneral = vr.EEEdoGeneral;
                    if (vr.EnergiaElec != null)
                    {
                        foreach (var i in vr.EnergiaElec)
                        {
                            switch (i)
                            {
                                case "Subestacion":
                                    ee.Subestacion = true;
                                    ee.SubestacionEstado = vr.EESubestacion;
                                    break;
                                case "MuroAcometida":
                                    ee.MuroAcometida = true;
                                    ee.MuroAcometidaEstado = vr.EEMuroAcometida;
                                    break;
                                case "AlumbradoExt":
                                    ee.AlumbradoExt = true;
                                    ee.AlumbradoExtEstado = vr.EEAlumbradoExt;
                                    break;
                                case "Luminaria":
                                    ee.Luminaria = true;
                                    ee.LuminariaEstado = vr.EELuminaria;
                                    break;
                                case "TableroDistribucion":
                                    ee.TableroDistribucion = true;
                                    ee.TableroDistribucionEstado = vr.EETableroDistribucion;
                                    break;
                                case "InterruptorGral":
                                    ee.InterruptorGral = true;
                                    ee.InterruptorGralEstado = vr.EEInterruptorGral;
                                    break;
                                case "Termomagnetico":
                                    ee.Termomagnetico = true;
                                    ee.TermomagneticoElectrico = vr.EETermomagnetico;
                                    break;
                            }
                        }
                    }
                    var contador2 = 0;
                    if (ee.IdEnergiaElectrica > 0)
                    {
                        if (ee.Subestacion != true) { ee.Subestacion = false; ee.SubestacionEstado = null; contador2 += 1; }
                        if (ee.MuroAcometida != true) { ee.MuroAcometida = false; ee.MuroAcometidaEstado = null; contador2 += 1; }
                        if (ee.AlumbradoExt != true) { ee.AlumbradoExt = false; ee.AlumbradoExtEstado = null; contador2 += 1; }
                        if (ee.Luminaria != true) { ee.Luminaria = false; ee.LuminariaEstado = null; contador2 += 1; }
                        if (ee.TableroDistribucion != true) { ee.TableroDistribucion = false; ee.TableroDistribucionEstado = null; contador2 += 1; }
                        if (ee.InterruptorGral != true) { ee.InterruptorGral = false; ee.InterruptorGralEstado = null; contador2 += 1; }
                        if (ee.Termomagnetico != true) { ee.Termomagnetico = false; ee.TermomagneticoElectrico = null; contador2 += 1; }
                        if (contador2 == 7) { ad.EstadoGeneral = null; }
                        ee.Editar();
                    }
                    #endregion

                    #region espacio multiple
                    //septimo modulo infraestructura de usos multiples
                    EspacioMultiple esm = vr.EspacioMultiple;
                    esm.IdValidarFk = vr.validacion.IdValidar;

                    if (vr.EspaciosMulti != null)
                    {
                        foreach (var i in vr.EspaciosMulti)
                        {
                            switch (i)
                            {
                                case "Barda":
                                    esm.Barda = true;
                                    esm.BardaEstado = vr.EMBarda;
                                    break;
                                case "Rampas":
                                    esm.Rampas = true;
                                    esm.RampasEstado = vr.EMRampas;
                                    break;
                                case "CanchaMultiple":
                                    esm.CanchaMultiple = true;
                                    esm.CanchaMultipleEstado = vr.EMCanchaMultiple;
                                    break;
                                case "Plaza":
                                    esm.Plaza = true;
                                    esm.PlazaEstado = vr.EMPlazaCivica;
                                    break;
                                case "DomoMalla":
                                    esm.DomoMalla = true;
                                    esm.DomoMallaEstado = vr.EMDomoMalla;
                                    break;
                                case "AstaBandera":
                                    esm.AstaBandera = true;
                                    esm.AstaBanderaEstado = vr.EMAstaBandera;
                                    break;
                                case "Bebedero":
                                    esm.Bebedero = true;
                                    esm.BebederoEstado = vr.EMBebedero;
                                    break;
                                case "AccesoP":
                                    esm.AccesoP = true;
                                    esm.AccesoPEstado = vr.EMAccesoP;
                                    break;
                                case "Areajuegos":
                                    esm.Areajuegos = true;
                                    esm.AreajuegosEstado = vr.EMAreajuegos;
                                    break;
                                case "Otro":
                                    esm.Otro = true;
                                    esm.OtroEstado = vr.EMOtro;
                                    break;
                            }
                        }
                    }

                    if (esm.IdEspacioMultiple > 0)
                    {
                        if (esm.Barda != true) { esm.Barda = false; esm.BardaEstado = null; }
                        if (esm.Rampas != true) { esm.Rampas = false; esm.RampasEstado = null; }
                        if (esm.CanchaMultiple != true) { esm.CanchaMultiple = false; esm.CanchaMultipleEstado = null; }
                        if (esm.Plaza != true) { esm.Plaza = false; esm.PlazaEstado = null; }
                        if (esm.DomoMalla != true) { esm.DomoMalla = false; esm.DomoMallaEstado = null; }
                        if (esm.AstaBandera != true) { esm.AstaBandera = false; esm.AstaBanderaEstado = null; }
                        if (esm.Bebedero != true) { esm.Bebedero = false; esm.BebederoEstado = null; }
                        if (esm.AccesoP != true) { esm.AccesoP = false; esm.AccesoPEstado = null; }
                        if (esm.Areajuegos != true) { esm.Areajuegos = false; esm.AreajuegosEstado = null; }
                        if (esm.Otro != true) { esm.Otro = false; esm.OtroEstado = null; }
                        esm.Editar();
                    }
                    #endregion

                    #region espacio educativo
                    //octavo modulo espacios educativos por modulos

                    EspacioEducativo ese = vr.EspacioEducativo;
                    EspacioEducativoDet.EliminaRegistros(ese.IdEspacioEducativo);

                    List<EspacioEducativoDet> eed = new List<EspacioEducativoDet>();

                    var control = vr.EEDEdificio.Count();
                    for(var i = 1; i < control; i++)
                    {
                        EspacioEducativoDet ex = new EspacioEducativoDet();
                        ex.IdEspacioEducativoFk = ese.IdEspacioEducativo;
                        ex.Edificio = i;
                        ex.TipoEstructura = vr.EEDTipologia[i];
                        ex.Aulas = vr.EEDAulas[i];
                        ex.Taller = vr.EEDTaller[i];
                        ex.ServicioSanitario = vr.EEDSanitario[i];
                        ex.ServicioAdministrativoBiblio = vr.EEDAdministrativo[i];
                        ex.Laboratirio = vr.EEDLaboratorio[i];
                        ex.BodegaApendice = vr.EEDBodegas[i];
                        ex.AulasUsoMultiple = vr.EEDAulaMultiple[i];

                        ex.Crear();
                    }
                    #endregion

                    #region  componente I
                    ComponenteI c1 = vr.ComponenteI;
                    c1.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.Matricula.IdMatricula > 0)
                    {
                        c1.Editar();
                    }
                    #endregion

                    #region  componente II
                    ComponenteII c2 = vr.ComponenteII;
                    c2.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.ComponenteII.IdComponenteII > 0)
                    {
                        c2.Editar();
                    }
                    #endregion

                    #region  componente III

                    var cont = vr.C3Conceptos.Count();
                    ComponenteIII.EliminaRegistros(vr.validacion.IdValidar);
                    for (var i = 0; i < cont; i++)
                    {
                        ComponenteIII c3 = new ComponenteIII();
                        c3.IdValidarFk = vr.validacion.IdValidar;
                        c3.Concepto = vr.C3Conceptos[i];
                        c3.Solicita = vr.C3Solicitante[i];
                        c3.Requiere = vr.C3Requerimientos[i];
                        c3.Crear();
                    }

                    #endregion

                    #region  componente VI
                    ComponenteIV c4 = vr.ComponenteIV;
                    c4.IdValidarFk = vr.validacion.IdValidar;
                    c4.Rehabilitacion = vr.CIVRehabilitacion;
                    if (vr.ComponenteIV.IdComponenteIV > 0)
                    {
                        c4.Editar();
                    }
                    #endregion

                    #region  componente V
                    ComponenteV c5 = vr.ComponenteV;
                    c5.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.ComponenteV.IdComponenteV > 0)
                    {
                        c5.Editar();
                    }
                    #endregion

                    #region  componente VI
                    ComponenteVI c6 = vr.ComponenteVI;
                    c6.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.ComponenteVI.IdComponenteVI > 0)
                    {
                        c6.Editar();
                    }
                    #endregion

                    #region  componente VII
                    ComponenteVII c7 = vr.ComponenteVII;
                    c7.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.ComponenteVII.IdComponenteVII > 0)
                    {
                        c7.Editar();
                    }
                    #endregion

                    #region  componente VIII
                    ComponenteVIII c8 = vr.ComponenteVIII;
                    c8.IdValidarFk = vr.validacion.IdValidar;
                    if (vr.ComponenteVIII.IdComponenteVIII > 0)
                    {
                        c8.Editar();
                    }
                    #endregion
                }

                #endregion
                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes/",
                    msj = "La validacion de la solicitud se ha completado correctamente"
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = true,
                    dir = "/Solicitudes/",
                    msj = "La validacion  no se ha canalizado correctamente"
                });
            }
        }

        public ActionResult Historial(int escuela)
        {
            SASEntities db = new SASEntities();
            List<ValidacionEscuela> v = db.Validacion
                                     .Where(x => x.IdEscuelaFk == escuela)
                                     .Select(x => new ValidacionEscuela {
                                         Ide = escuela,
                                         Idv = x.IdValidar,
                                         fecha = x.FechaValidacion.Value.ToString(),
                                         Historial = x.Historial != true ? false : true,
                                         nombreUsuario = x.IdUsario
                                     })
                                     .OrderBy(x => x.fecha)
                                     .ToList(); 
            return View(v);
        }

        public FileContentResult GetCroquis(int? id)
        {
            SASEntities db = new SASEntities();
            var croquis = db.Croquis.FirstOrDefault(i => i.IdCroquis == id);
            if (croquis != null)
            {

                string type = string.Empty;
                type = croquis.Tipo;
                var file = File(croquis.DocCroquis, type);
                return file;
            }
            else
            {
                return null;
            }
        }

        public JsonResult PutCroquis(int IdCroquis, HttpPostedFileBase file)
        {
            try
            {
                SASEntities db = new SASEntities();
                Croquis c = db.Croquis.FirstOrDefault(i => i.IdCroquis == IdCroquis);

                if (file != null)
                {
                    int length = file.ContentLength;
                    byte[] buffer = new byte[length];
                    file.InputStream.Read(buffer, 0, length);
                    c.DocCroquis = buffer;
                    c.Tipo = file.ContentType;
                    c.EditarDocumento();
                }

                return Json(new
                {
                    result = true,
                    msj = "El croquis se a guardado correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = false,
                    msj = "El documento no se a podido guardar, intentalo nuevamente"
                });
            }
        }

        public ActionResult Escuelas()
        {
            SASEntities db = new SASEntities();
            List<EscuelaValidacion> Escuelas = db.Escuela
                                                 .Select(x => new EscuelaValidacion {
                                                     Id = x.IdEscuela,
                                                     Nombre = x.Nombre,
                                                     Clave = x.Clave,
                                                     TotalValidaciones = x.Validacion
                                                                          .Where(z => z.IdEscuelaFk == x.IdEscuela)
                                                                          .Count(),
                                                     UltimaValidacion = db.Validacion
                                                                          .FirstOrDefault(z =>
                                                                                          z.IdEscuelaFk == x.IdEscuela &&
                                                                                          z.Historial == false) != null ?                                           db.Validacion
                                                                          .FirstOrDefault(z => 
                                                                                          z.IdEscuelaFk == x.IdEscuela &&
                                                                                          z.Historial == false)
                                                                          .FechaValidacion.ToString() : ""
                                                 }).ToList();
            return View(Escuelas);
        }

        public JsonResult CerrarValidacion(int id)
        {
            try
            {
                SASEntities db = new SASEntities();
                Validacion v = db.Validacion.FirstOrDefault(x => x.IdValidar == id);
                v.Historial = true;

                db.SaveChanges();

                return Json(new
                {
                    result = true,
                    dir = "/Validaciones/",
                    msj = "La validacion ha sido cerrada correctamente"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = true,
                    dir = "/Validaciones/",
                    msj = "La validacion  no se ha cerrado correctamente"
                });
            }
        }
    }
}