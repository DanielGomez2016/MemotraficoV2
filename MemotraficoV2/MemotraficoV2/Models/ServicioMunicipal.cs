using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class ServicioMunicipal
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.ServicioMunicipal.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            ServicioMunicipal sm = db.ServicioMunicipal.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            sm.RedAgua = RedAgua;
            sm.RedAguaDistancia = RedAguaDistancia;
            sm.RedDrenaje = RedDrenaje;
            sm.RedDrenajeDistancia = RedDrenajeDistancia;
            sm.RedEnergia = RedEnergia;
            sm.RedEnergiaDistancia = RedEnergiaDistancia;
            sm.RedAlumbrado = RedAlumbrado;
            sm.RedAlumbradoDistancia = RedAlumbradoDistancia;
            sm.VialidadPrimaria = VialidadPrimaria;
            sm.VialidadSecundaria = VialidadSecundaria;
            sm.VialidadTerciaria = VialidadTerciaria;
            db.SaveChanges();
        }

        public static ServicioMunicipal ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            ServicioMunicipal sm = new ServicioMunicipal();
            var registro = db.ServicioMunicipal.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                sm.IdValidarFk = valor;
                sm.RedAgua = false;
                sm.RedAguaDistancia = "";
                sm.RedDrenaje = false;
                sm.RedDrenajeDistancia = "";
                sm.RedEnergia = false;
                sm.RedEnergiaDistancia = "";
                sm.RedAlumbrado = false;
                sm.RedAlumbradoDistancia = "";
                sm.VialidadPrimaria = false;
                sm.VialidadSecundaria = false;
                sm.VialidadTerciaria = false;
                sm.Crear();
            }
            sm = db.ServicioMunicipal.FirstOrDefault(i => i.IdValidarFk == valor);
            return sm;
        }
    }
}