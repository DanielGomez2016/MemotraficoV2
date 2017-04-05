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
        public Matricula Matricula { get; set; }



        //Energia electrica
        public EnergiaElectrica EnergiaElectrica { get; set; }
        public string[] EnergiaElec { get; set; }
        public int EESubestacion { get; set; }
        public int EEMuroAcometida { get; set; }
        public int EEAlumbradoExt { get; set; }
        public int EELuminaria { get; set; }
        public int EETableroDistribucion { get; set; }
        public int EEInterruptorGral { get; set; }
        public int EETermomagnetico { get; set; }
        public int EEEdoGeneral { get; set; }



        // entorno
        public Entorno Entorno { get; set; }
        public int EntornoInfraestructura { get; set; }
        public int EntornoTerreno { get; set; }
        public string[] EntornoAmenaza { get; set; }

        

        //Croquis
        public Croquis Croquis { get; set; }
        public int CroquisTipoSuelo { get; set; }
        public int CroquisPredio { get; set; }



        //Servicio municipal
        public ServicioMunicipal ServicioMunicipal { get; set; }
        public string[] SMVialidades { get; set; }
        public string[] SMRed { get; set; }



        //almacenamientoDren
        public AlmacenamientoDren AlmacenamientoDren { get; set; }
        public string[] AlmanceDren { get; set; }
        public int ADAguaPotable { get; set; }
        public int ADPipas { get; set; }
        public int ADCisterna { get; set; }
        public int ADTinaco { get; set; }
        public int ADColectorM { get; set; }
        public int ADFosaSeptica { get; set; }
        public int ADEdoGeneral { get; set; }



        //Espacios Educativos
        public EspacioEducativo EspacioEducativo { get; set; }
        public EspacioEducativoDet[] EspacioEducativoDet { get; set; }
        public int[] EEDEdificio { get; set; }
        public int[] EEDTipologia { get; set; }
        public int[] EEDAulas { get; set; }
        public int[] EEDLaboratorio { get; set; }
        public int[] EEDSanitario { get; set; }
        public int[] EEDAdministrativo { get; set; }
        public int[] EEDBiblioteca { get; set; }
        public int[] EEDAulaMultiple { get; set; }
        public int[] EEDBodegas { get; set; }



        //Espacios multiples
        public EspacioMultiple EspacioMultiple { get; set; }
        public string[] EspaciosMulti { get; set; }
        public int EMBarda { get; set; }
        public int EMRampas { get; set; }
        public int EMCanchaMultiple { get; set; }
        public int EMPlazaCivica { get; set; }
        public int EMDomoMalla { get; set; }
        public int EMAstaBandera { get; set; }
        public int EMBebedero { get; set; }
        public int EMAccesoP { get; set; }
        public int EMAreajuegos { get; set; }
        public int EMOtro { get; set; }




        public int requerimientos { get; set; }
        public ComponenteI ComponenteI { get; set; }
        public ComponenteII ComponenteII { get; set; }


        //componente III
        public ComponenteIII[] ComponenteIII { get; set; }
        public string[] C3Conceptos { get; set; }
        public int[] C3Solicitante { get; set; }
        public int[] C3Requerimientos { get; set; }


        //Componente IV
        public ComponenteIV ComponenteIV { get; set; }
        public int CIVRehabilitacion { get; set; }




        public ComponenteV ComponenteV { get; set; }
        public ComponenteVI ComponenteVI { get; set; }
        public ComponenteVII ComponenteVII { get; set; }
        public ComponenteVIII ComponenteVIII { get; set; }

    }
}