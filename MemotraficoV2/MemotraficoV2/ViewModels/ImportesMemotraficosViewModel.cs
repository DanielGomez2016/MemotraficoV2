using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class memotraficos
    {
        public long idmemotrafico { get; set; }
        public string memotrafico { get; set; }
        public string leido { get; set; }
        public string fechaCreado { get; set; }
        public string asunto { get; set; }
        public long idestatus { get; set; }
        public string estatus { get; set; }
        public string fecharecibido { get; set; }
        public string procedencia { get; set; }
        public string tipoasunto { get; set; }
        public string beneficiario { get; set; }
        public string cerrado { get; set; }
        public string cancelado { get; set; }
    }

    public class memotraficoImport{
        public int idmemotrafico { get; set; }
        public string folio { get; set; }
        public DateTime fechaEntrega { get; set; }
        public string asunto { get; set; }
        public int idestatus { get; set; }
        public EsProcedencia procedencia { get; set; }
        public int idasunto { get; set; }
        public EsEscuela escuela { get; set; }
        public bool cerrado { get; set; }
        public bool cancelado { get; set; }
        public DateTime? fechaCanalizado { get; set; }
        public DateTime? fechaCancelado { get; set; }
        public DateTime? fechaDocumento { get; set; }
        public int tiporespuesta { get; set; }
        public DocumentoMemo documentomemo { get; set; }
        public List<canalizacionMemo> canalizaciones { get; set; }

        
    }

    public class DocumentoMemo
    {
        public string descripcion { get; set; }
        public byte[] Documento { get; set; }
        public string nombre { get; set; }
    }

    public class canalizacionMemo
    {
        public int idCanalizacion { get; set; }
        public string deptoenvia { get; set; }
        public string deptorecibe { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public string comentario { get; set; }
        public List<documentoCanalizacion> documentoCanalizacion { get; set; }
    }

    public class documentoCanalizacion
    {
        public string descripcion { get; set; }
        public byte[] Documento { get; set; }
        public string nombre { get; set; }
    }

    public class EsEscuela
    {
        public int id { get; set; }
        public bool nombre { get; set; }

    }

    public class EsProcedencia
    {
        public int idporcedencia { get; set; }
        public int idtipo { get; set; }
    }

    public class beneficiarios
    {
        public long idbeneficiario { get; set; }
        public string beneficiario { get; set; }
        public string clave { get; set; }
        public string domicilio { get; set; }
        public long? localidad { get; set; }
        public long? municipio { get; set; }
        public string director { get; set; }
        public long? idniveleducativo { get; set; }
        public string telefono { get; set; }
        public string tipotelefono { get; set; }

    }

    public class beneficiariosImport
    {
        public long idbeneficiario { get; set; }
        public string beneficiario { get; set; }
        public string clave { get; set; }
        public string domicilio { get; set; }
        public int localidad { get; set; }
        public int municipio { get; set; }
        public string director { get; set; }
        public int idniveleducativo { get; set; }
        public string telefono { get; set; }
        public string tipotelefono { get; set; }
        public bool EsEscuela { get; set; }

    }

    public class procedencia
    {
        public long idprocedencia { get; set; }
        public string nombre { get; set; }
        public string domicilio { get; set; }
        public long idmunicipio { get; set; }
        public long idlocalidad { get; set; }
        public string contacto { get; set; }
        public long idtipoprocedencia { get; set; }
        public string tipoprocedencia { get; set; }

    }

    public class procedenciaImport
    {
        public string procedencia { get; set; }
        public string domicilio { get; set; }
        public int municipio { get; set; }
        public int localidad { get; set; }
        public string contacto { get; set; }
        public string tipoprocedencia{ get; set; }
    }
}
