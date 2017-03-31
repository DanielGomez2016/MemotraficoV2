using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemotraficoV2.Models;

namespace MemotraficoV2.ViewModels
{
    public class Validacion_Requerimientos
    {
        public int aulas { get; set; }
        public int laboratorios { get; set; }
        public int talleres { get; set; }
        public int anexos { get; set; }
        public string FolioSolicitud { get; set; }

        public Escuela Escuela { get; set; }
        public Contacto Contacto { get; set; }

        public int validacion { get; set; }
        public EnergiaElectrica EnergiaElectrica { get; set; }
        public Entorno Entorno { get; set; }
        public Croquis Croquis { get; set; }
        public ServicioMunicipal ServicioMunicipal { get; set; }
        public AlmacenamientoDren AlmacenamientoDren { get; set; }
        public EspacioEducativo EspacioEducativo { get; set; }
        public EspacioEducativoDet EspacioEducativoDet { get; set; }
        public EspacioMultiple EspacioMultiple { get; set; }

        public int requerimientos { get; set; }
        public ComponenteI ComponenteI { get; set; }
        public ComponenteII ComponenteII { get; set; }
        public ComponenteIII ComponenteIII { get; set; }
        public ComponenteIV ComponenteIV { get; set; }
        public ComponenteV ComponenteV { get; set; }
        public ComponenteVI ComponenteVI { get; set; }
        public ComponenteVII ComponenteVII { get; set; }
        public ComponenteVIII ComponenteVIII { get; set; }

    }
}