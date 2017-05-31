using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class EspacioMultiple
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.EspacioMultiple.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            EspacioMultiple ep = db.EspacioMultiple.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            ep.Areajuegos = Areajuegos;
            ep.AreajuegosEstado = AreajuegosEstado;
            ep.Barda = Barda;
            ep.BardaEstado = BardaEstado;
            ep.Rampas = Rampas;
            ep.RampasEstado = RampasEstado;
            ep.CanchaMultiple = CanchaMultiple;
            ep.CanchaMultipleEstado = CanchaMultipleEstado;
            ep.Plaza = Plaza;
            ep.PlazaEstado = PlazaEstado;
            ep.DomoMalla = DomoMalla;
            ep.DomoMallaEstado = DomoMallaEstado;
            ep.Bebedero = Bebedero;
            ep.BebederoEstado = BebederoEstado;
            ep.AccesoP = AccesoP;
            ep.AccesoPEstado = AccesoPEstado;
            ep.AstaBandera = AstaBandera;
            ep.AstaBanderaEstado = AstaBanderaEstado;
            ep.Otro = Otro;
            ep.OtroEstado = OtroEstado;
            db.SaveChanges();
        }

        public static EspacioMultiple ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            EspacioMultiple ep = new EspacioMultiple();
            var registro = db.EspacioMultiple.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                ep.IdValidarFk = valor;
                ep.Areajuegos = false;
                ep.AreajuegosEstado = 0;
                ep.Barda = false;
                ep.BardaEstado = 0;
                ep.Rampas = false;
                ep.RampasEstado = 0;
                ep.CanchaMultiple = false;
                ep.CanchaMultipleEstado = 0;
                ep.Plaza = false;
                ep.PlazaEstado = 0;
                ep.DomoMalla = false;
                ep.DomoMallaEstado = 0;
                ep.Bebedero = false;
                ep.BebederoEstado = 0;
                ep.AccesoP = false;
                ep.AccesoPEstado = 0;
                ep.AstaBandera = false;
                ep.AstaBanderaEstado = 0;
                ep.Otro = false;
                ep.OtroEstado = 0;
                ep.Crear();
            }
            ep = db.EspacioMultiple.FirstOrDefault(i => i.IdValidarFk == valor);
            return ep;
        }
    }
}