using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemotraficoV2.Models;

namespace MemotraficoV2.ViewModels
{
    public class SolicitudDetalle
    {
        //Datos de solicitud
        public Solicitudes solicitud { get; set; }

        //historial de la solicitud
        public List<Detalle> Detalles { get; set; }

        //institucion que tiene asignada la solicitud
        public string institucion { get; set; }
    }

    //datos concretos de cada uno de los detalles
    public class Detalle
    {
        //Fecha de la canalizacion ordenada de mayor a menor
        public string FechaCanalizado { get; set; }
        //comentario que se realizo
        public string Comentario { get; set; }
        //usuario que hizo la canalizacion
        public string usuario { get; set; }
        //departamento que realizo algun 
        public string departamento { get; set; }
        //usuario al que se le canalizo la solicitud
        public string usuarioatiende { get; set; }
        //estatus en el que se encontraba la solicitud
        public string estatus { get; set; }
        //color del registro
        public string colorreg { get; set; }
        public int numregistro { get; set; }
    }
}