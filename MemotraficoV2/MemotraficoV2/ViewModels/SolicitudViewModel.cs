using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class SolicitudGrid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string folio { get; set; }
        public string fecha { get; set; }
        public int estatus { get; set; }
        public int? nivel { get; set; }
        public string colornivel { get; set; }

    }
}