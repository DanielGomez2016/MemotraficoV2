using MemotraficoV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class SeguimientoSolicitud
    {
        public Escuela Escuela { get; set; }
        public Solicitudes Solicitud { get; set; }
        public Canalizacion Canalizacion { get; set; }
        public List<Detalle> DetCanalizacion { get; set; }
    }
}