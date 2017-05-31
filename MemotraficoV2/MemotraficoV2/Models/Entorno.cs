using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Entorno
    {
        public void Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Entorno e = db.Entorno.FirstOrDefault(i => i.IdEntorno == IdEntorno);
                e.Infraestructura = Infraestructura != null ? Infraestructura : e.Infraestructura;
                e.Terreno = Terreno != null ? Terreno : e.Terreno;
                e.Rio_Arrollo = Rio_Arrollo;
                e.Rio_ArrolloDistancia = Rio_ArrolloDistancia != null ? Rio_ArrolloDistancia : e.Rio_ArrolloDistancia;
                e.AmenazaVial = AmenazaVial;
                e.AmenazaVialDistancia = AmenazaVialDistancia != null ? AmenazaVialDistancia : e.AmenazaVialDistancia;
                e.Comercio = Comercio;
                e.ComercioDistancia = ComercioDistancia != null ? ComercioDistancia : e.ComercioDistancia;
                e.DerechoVia = DerechoVia;
                e.DerechoViaDistancia = DerechoViaDistancia != null ? DerechoViaDistancia : e.DerechoViaDistancia;
                e.Gasolinera = Gasolinera;
                e.GasolineraDistancia = GasolineraDistancia != null ? GasolineraDistancia : e.GasolineraDistancia;

                db.SaveChanges();
            }
            catch { }
        }
        public void Crear()
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Entorno.AddObject(this);
                db.SaveChanges();
            }
            catch { }
        }

        public static Entorno ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            Entorno e = new Entorno();
            var registro = db.Entorno.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                e.IdValidarFk = valor;
                e.Rio_Arrollo = false;
                e.Rio_ArrolloDistancia = "";
                e.AmenazaVial = false;
                e.AmenazaVialDistancia = "";
                e.Comercio = false;
                e.ComercioDistancia = "";
                e.DerechoVia = false;
                e.DerechoViaDistancia = "";
                e.Gasolinera = false;
                e.GasolineraDistancia = "";
                e.Crear();
            }
            e = db.Entorno.FirstOrDefault(i => i.IdValidarFk == valor);
            return e;
        }
    }
}