using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.ViewModels
{
    public class EscuelaValidacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public int TotalValidaciones { get; set; }
        public string UltimaValidacion { get; set; }
    }

    public class ValidacionEscuela
    {
        public int Ide { get; set; }
        public int Idv { get; set; }
        public string fecha { get; set; }
        public bool Historial { get; set; }
        public string nombreUsuario { get; set; }
    }
}