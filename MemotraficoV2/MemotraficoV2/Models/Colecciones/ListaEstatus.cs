﻿using System;
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
        public const string CanalizadaDependencia = "Se canalizo a la dependencia indicada";
        public const string CanalizadaOperador = "Se canalizo al operador asignado";
        public const string CanalizadaOxO = "Se canalizo a otro operador para continuar con la resolucion de la solicitud";
        public const string Cancelacion = "Se cancelo la solicitud indicada";
        public const string Cerrada = "La solicitud fue cerrada";
        public const string ReAbrir = "La solicitud fue abierta nuevamente";
        public const string Avance = "Avance de solicitud, con documento(s)";

    }
}