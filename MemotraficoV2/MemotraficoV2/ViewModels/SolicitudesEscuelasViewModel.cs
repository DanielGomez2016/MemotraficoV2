using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class SolicitudesEscuelasViewModel
    {
        public int idEscuela { get; set; }
        public string escuela { get; set; }
        public string director { get; set; }
        public string localidad { get; set; }
        public int Solicitudes { get; set; }
    }

    public class SolicitudesViewModel
    {
        public string Folio { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaEntrega { get; set; }

    }
}