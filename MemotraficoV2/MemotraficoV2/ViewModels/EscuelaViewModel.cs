using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class escuelas
    {
        public long id { get; set; }
        public string nombre { get; set; }
        public string clave { get; set; }
        public string nivel { get; set; }
        public string director { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string localidad { get; set; }
    }
}
