using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Departamento
    {
        public class mEmail
        {
            public int IdEmail { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public string IdUser { get; set; }
        }
    }
}