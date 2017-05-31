using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class AlmacenamientoDren
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.AlmacenamientoDren.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            AlmacenamientoDren ad = db.AlmacenamientoDren.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            ad.Redagua = Redagua;
            ad.RedaguaEstado = RedaguaEstado;
            ad.Pipa = Pipa;
            ad.PipaEstado = PipaEstado;
            ad.Cisterna = Cisterna;
            ad.CisternaEstado = CisternaEstado;
            ad.Tinaco = Tinaco;
            ad.TinacoEstado = TinacoEstado;
            ad.ColectorMunicipal = ColectorMunicipal;
            ad.ColectorMunicipalEstado = ColectorMunicipalEstado;
            ad.FosaSeptica = FosaSeptica;
            ad.FosaSepticaEstado = FosaSepticaEstado;
            ad.EstadoGeneral = EstadoGeneral;
            db.SaveChanges();
        }

        public static AlmacenamientoDren ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            AlmacenamientoDren ad = new AlmacenamientoDren();
            var registro = db.AlmacenamientoDren.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                ad.IdValidarFk = valor;
                ad.Redagua = false;
                ad.RedaguaEstado = 0;
                ad.Pipa = false;
                ad.PipaEstado = 0;
                ad.Cisterna = false;
                ad.CisternaEstado = 0;
                ad.Tinaco = false;
                ad.TinacoEstado = 0;
                ad.ColectorMunicipal = false;
                ad.ColectorMunicipalEstado = 0;
                ad.FosaSeptica = false;
                ad.FosaSepticaEstado = 0;
                ad.EstadoGeneral = 0;
                ad.Crear();
            }
            ad = db.AlmacenamientoDren.FirstOrDefault(i => i.IdValidarFk == valor);
            return ad;
        }
    }
}