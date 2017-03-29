using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models.Colecciones
{
    public class ListaEstatus
    {
        public const int INICIADO = 1;
        public const int CANALIZADO = 2;
        public const int CERRADO = 3;
        public const int CANCELADO = 4;
    }

    public class ListaValidaciones
    {
        public const bool SI_VALIDACION = true;
        public const bool NO_VALIDACION = false;
    }

    public class ListaComentarios
    {
        public const string INICIADA = "Se inicializo solicitud con el folio ";
    }
}