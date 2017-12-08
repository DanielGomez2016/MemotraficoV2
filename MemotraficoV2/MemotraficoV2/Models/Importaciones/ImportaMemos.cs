using MemotraficoV2.Models.Colecciones;
using MemotraficoV2.Models.ConexionesDb;
using MemotraficoV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models.Importaciones
{
    public class ImportaMemos
    {
        #region Memotraficos
        public static AnexGRIDResponde AllMemotraficos(AnexGRID grid)
        {
            grid.Inicializar();

            try
            {
                MemotraficosEntities db = new MemotraficosEntities();

                var memos = db.Memotraficos.AsQueryable();

                if (grid.columna == "memotrafico")
                {
                    memos = grid.columna_orden == "DESC" ? memos.OrderBy(x => x.IdMemotrafico)
                                                        : memos.OrderByDescending(x => x.IdMemotrafico);
                }

                foreach (var f in grid.filtros)
                {
                    if (f.columna == "memotrafico")
                        memos = memos.Where(x => x.Memotrafico.Contains(f.valor));
                    if (f.columna == "fechaCreado")
                        memos = memos.Where(x => x.Fecha.ToString().Contains(f.valor));
                }

                var query = memos.Select(x => new memotraficos
                {
                    idmemotrafico = x.IdMemotrafico,
                    memotrafico = x.Memotrafico,
                    leido = x.Leido == true ? "Leido" : "En espera",
                    fechaCreado = x.Fecha.ToString().Substring(0,11),
                    asunto = x.Asunto,
                    idestatus = x.IdEstatusMemo,
                    estatus = x.EstatusMemos.Estatus,
                    fecharecibido = x.FechaRecibido.ToString().Substring(0, 11),
                    procedencia = x.Procedencias.Procedencia,
                    tipoasunto = x.TipoAsuntos.Asunto,
                    beneficiario = x.Beneficiarios.Beneficiario,
                    cerrado = x.Cerrado == true ? "Cerrado" : "",
                    cancelado = x.Cancelado == true ? "Cancelado" : "",
                }).ToList();

                var data = query
                           .Skip(grid.pagina)
                           .Take(grid.limite)
                           .ToList();

                var total = query.Count();

                grid.SetData(data, total);
            }
            catch (Exception e)
            {

            }

            return grid.responde();
        }

        public static int TipoRespuesta(bool? r1,bool? r2, bool? r3, bool? r4, bool? r5, bool? r6, bool? r7)
        {
            var result = 0;
            try{
                SASEntities db = new SASEntities();

                if (r1 != null && r1 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 1).IdRespuesta; }
                if (r2 != null && r2 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 2).IdRespuesta; }
                if (r3 != null && r3 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 3).IdRespuesta; }
                if (r4 != null && r4 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 4).IdRespuesta; }
                if (r5 != null && r5 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 5).IdRespuesta; }
                if (r6 != null && r6 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 6).IdRespuesta; }
                if (r7 != null && r7 == true) { result = db.TipoRespuesta.FirstOrDefault(x => x.IdRespuesta == 7).IdRespuesta; }


            }
            catch(Exception e)
            {
                
            }
            return result;
        }

        public static EsEscuela EsEscuela(string id, string nombre)
        {
            var result = new EsEscuela();
            try
            {
                SASEntities db = new SASEntities();
                var escuela = db.Escuela.FirstOrDefault(x => x.Clave.Contains(id));
                if (escuela != null) { 
                    result.id = escuela.IdEscuela;
                    result.nombre = true;
                }
                else
                {
                    var beneficiario = db.Beneficiario.FirstOrDefault(x => x.Nombre.Contains(nombre));
                    if (beneficiario != null)
                    {
                        result.id = beneficiario.IdBeneficiario;
                        result.nombre = false;
                    }
                }

            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static EsProcedencia Procedencia(string nombre)
        {
            var result = new EsProcedencia();
            SASEntities db = new SASEntities();
            try
            {
                var procedencia = db.Procedencia.Any(x => x.Procedencia1.Contains(nombre));
                if (procedencia)
                {
                    result.idporcedencia = db.Procedencia.FirstOrDefault(x => x.Procedencia1.Contains(nombre)).IdProcedencia;

                    result.idtipo = db.Procedencia.FirstOrDefault(x => x.Procedencia1.Contains(nombre)).TipoProcedencia.IdTipoProcedencia;
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static int TipoAsunto(string asunto)
        {
            var result = 0;
            SASEntities db = new SASEntities();
            try
            {
                var a = db.TipoAsunto.Any(x => x.TipoAsunto1.Contains(asunto));
                if (a)
                {
                    result = db.TipoAsunto.FirstOrDefault(x => x.TipoAsunto1.Contains(asunto)).IdTipoAsunto;
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static bool Importar(memotraficoImport m)
        {
            var result = false;
            SASEntities db = new SASEntities();
            try
            {
                if (m != null)
                {
                    //creamos la solicitud que se esta importando
                    Solicitudes s = new Solicitudes();
                    s.Folio = m.folio;
                    s.FechaEntrega = m.fechaEntrega;
                    s.Asunto = m.asunto;
                    s.IdProcedenciaFk = m.procedencia.idporcedencia;
                    s.IdTipoProcedenciaFk = m.procedencia.idtipo;
                    s.IdTipoAsuntoFk = m.idasunto;
                    if (m.escuela.nombre) {
                        s.IdEscuelaFk = m.escuela.id;
                    }else
                    {
                        s.IdBeneficiarioFk = m.escuela.id;
                    }

                    var idsolicitud = s.CrearImporte();
                    var idcanalizacion = 0;
                    var cont2 = 0;
                    var cont3 = 1;
                    var estatusfinal = 0;
                    var nocanalizaciones = m.canalizaciones != null ? m.canalizaciones.Count() : 0;
                    foreach (var i in m.canalizaciones)
                    {

                        var cont = 0;
                        if (!db.Canalizacion.Any(x => x.IdSolicitudFk == idsolicitud))
                        {
                            Canalizacion c = new Canalizacion();
                            c.IdSolicitudFk = idsolicitud;
                            c.IdInstitucionFk = Institucion.getinstitucionxsiglas("ICHIFE");
                            c.Validacion = false;

                            idcanalizacion = c.Crear();
                            cont = 1;
                        }

                        DetalleCanalizacion dc = new DetalleCanalizacion();
                        dc.IdCanalizarFk = idcanalizacion;
                        dc.FechaCanalizar = i.fechaInicio;
                        dc.Comentario = i.comentario;
                        dc.IdUsuarioFk = Usuarios.GetUsuarioIdxNombre(i.deptoenvia);
                        dc.Instituto = Institucion.getinstitucionxsiglas("ICHIFE");
                        dc.Departamento = Departamento.getxNombre(i.deptorecibe);
                        dc.UsuarioAtiende = Usuarios.GetUsuarioIdxNombre(i.deptorecibe);
                        estatusfinal = cont == 1 ? ListaEstatus.INICIADO : Util.getEstatus(m.idestatus, m.cerrado, m.cancelado, cont3, nocanalizaciones);
                        dc.IdEstatusFk = estatusfinal;
                        dc.IdRespuestaFk = m.tiporespuesta;

                         var sdc = dc.Crear();

                        if(m.documentomemo != null && cont2 == 0)
                        {
                            cont2 = 1;
                            Documentos doc = new Documentos();
                            doc.IdDetalleCanalizarFk = sdc;
                            doc.Nombre = m.documentomemo.nombre;
                            doc.Documento = m.documentomemo.Documento;
                            doc.Tipo = "application/pdf";

                            doc.CrearDoc();
                        }
                        if (i.documentoCanalizacion != null)
                        {
                            foreach (var n in i.documentoCanalizacion)
                            {
                                Documentos doc = new Documentos();
                                doc.IdDetalleCanalizarFk = sdc;
                                doc.Nombre = n.nombre;
                                doc.Documento = n.Documento;
                                doc.Tipo = "application/pdf";

                                doc.CrearDoc();
                            }
                        }
                        cont3++;

                    }
                    if (nocanalizaciones == (cont3-1))
                    {
                        Solicitudes ss = new Solicitudes();
                        ss = db.Solicitudes.FirstOrDefault(x => x.IdSolicitud == idsolicitud);
                        ss.IdEstatusFk = estatusfinal;

                        ss.EditarEstatus();
                    }
                    result = true;
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        #endregion

        #region Beneficiarios

        public static AnexGRIDResponde AllBeneficiarios(AnexGRID grid)
        {
            grid.Inicializar();

            try
            {
                MemotraficosEntities db = new MemotraficosEntities();

                var beneficiario = db.Beneficiarios.AsQueryable();

                if (grid.columna == "beneficiario")
                {
                    beneficiario = grid.columna_orden == "DESC" ? beneficiario.OrderBy(x => x.IdBeneficiario)
                                                        : beneficiario.OrderByDescending(x => x.IdBeneficiario);
                }

                foreach (var f in grid.filtros)
                {
                    if (f.columna == "beneficiario")
                        beneficiario = beneficiario.Where(x => x.Beneficiario.Contains(f.valor));

                    if (f.columna == "clave")
                        beneficiario = beneficiario.Where(x => x.clave.Contains(f.valor));
                }

                var query = beneficiario.Select(x => new beneficiarios
                {
                    idbeneficiario = x.IdBeneficiario,
                    beneficiario = x.Beneficiario,
                    clave = x.clave,
                    domicilio = x.Domicilio,
                    director = x.Director,
                    telefono = db.TelefonosBeneficiarios.FirstOrDefault(z => z.IdBeneficiario == x.IdBeneficiario).Telefono.ToString(),

                }).ToList();

                var data = query
                           .Skip(grid.pagina)
                           .Take(grid.limite)
                           .ToList();

                var total = query.Count();

                grid.SetData(data, total);
            }
            catch (Exception e)
            {

            }

            return grid.responde();
        }

        public static bool ImportarBeneficiario(beneficiariosImport b)
        {
            var result = false;
            SASEntities db = new SASEntities();
            try
            {
                Escuela e = new Escuela();
                Beneficiario _b = new Beneficiario();
                var idesc = 0;
                var idben = 0;

                if (b.EsEscuela) {
                    e.Clave = b.clave;
                    e.IdNivelEducativo = b.idniveleducativo;

                    idesc = e.EditarNivel();

                    //Contacto c = new Contacto();
                    //c.IdEscuelaFk = idesc;
                    //c.Nombre = b.director;

                    //if(b.tipotelefono == "OFICINA") {
                    //    c.Telefono = b.telefono;
                    //} else {
                    //    c.Celular = b.telefono;
                    //}

                    //c.Crear();
                    result = true;
                    
                }
                else
                {
                    _b.Nombre = b.beneficiario;
                    _b.Domicilio = b.domicilio;
                    _b.IdLocalidadFk = b.localidad;
                    _b.IdMunicipioFk = b.municipio;

                    if (b.tipotelefono == "OFICINA")
                    {
                        _b.Telefono = b.telefono;
                    }
                    else
                    {
                        _b.Celular = b.telefono;
                    }

                    idben = _b.Crear();

                    result = true;
                }
               
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static int IdMunicipio(string municipio)
        {
            var result = 19;
            SASEntities db = new SASEntities();
            try
            {
                result = db.Municipios.FirstOrDefault(x => x.Nombre.Contains(municipio)) != null ? db.Municipios.FirstOrDefault(x => x.Nombre.Contains(municipio)).IdMunicipio : 1;
            }
            catch (Exception e) {

            }
            return result;
        }

        public static int IdLocalidad(string localidad)
        {
            var result = 1;
            SASEntities db = new SASEntities();
            try
            {
                if (localidad != "")
                {
                    result = db.Localidades.FirstOrDefault(x => x.Nombre.Contains(localidad)) != null ? db.Localidades.FirstOrDefault(x => x.Nombre.Contains(localidad)).IdLocalidad : 1;
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static int NivelEducativo(string nivel)
        {
            var result = 0;
            SASEntities db = new SASEntities();
            try
            {
                result = db.NivelEducativo.FirstOrDefault(x => x.Nivel.Contains(nivel)).IdNivelEducativo;
            }
            catch (Exception e)
            {

            }
            return result;
        }

        #endregion

        #region Procedencia

        public static AnexGRIDResponde AllProcedencia(AnexGRID grid)
        {
            grid.Inicializar();

            try
            {
                MemotraficosEntities db = new MemotraficosEntities();

                var procedencia = db.Procedencias.AsQueryable();

                if (grid.columna == "procedencia")
                {
                    procedencia = grid.columna_orden == "DESC" ? procedencia.OrderBy(x => x.IdProcedencia)
                                                        : procedencia.OrderByDescending(x => x.IdProcedencia);
                }

                foreach (var f in grid.filtros)
                {
                    if (f.columna == "nombre")
                        procedencia = procedencia.Where(x => x.Procedencia.Contains(f.valor));

                    if (f.columna == "tipoprocedencia")
                        procedencia = procedencia.Where(x => x.TipoProcedencias.TipoProcedencia.Contains(f.valor));
                }

                var query = procedencia.Select(x => new procedencia
                {
                    idprocedencia = x.IdProcedencia,
                    nombre = x.Procedencia,
                    contacto = x.Contacto,
                    domicilio = x.Domicilio,
                    tipoprocedencia = db.TipoProcedencias.FirstOrDefault(z => z.IdTipoProcedencia == x.IdTipoProcedencia).TipoProcedencia.ToString(),

                }).ToList();

                var data = query
                           .Skip(grid.pagina)
                           .Take(grid.limite)
                           .ToList();

                var total = query.Count();

                grid.SetData(data, total);
            }
            catch (Exception e)
            {

            }

            return grid.responde();
        }

        public static bool ImportarProcedencia(procedenciaImport p)
        {
            var result = false;
            SASEntities db = new SASEntities();
            try
            {
                Procedencia _p = new Procedencia();

                _p.Procedencia1 = p.procedencia;
                _p.Domicilio = p.domicilio;
                _p.Municipio = p.municipio;
                _p.Localidad = p.localidad;
                _p.Contacto = p.contacto;
                _p.IdTipoProcedenciaFk = ImportaMemos.TipoProcedencia(p.tipoprocedencia);

                _p.Crear();

                result = true;   
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static int? TipoProcedencia(string tipoprocedencia)
        {
            SASEntities db = new SASEntities();

            return db.TipoProcedencia.FirstOrDefault(x => x.TipoProcedencia1 == tipoprocedencia).IdTipoProcedencia;
        }


        #endregion

    }
}