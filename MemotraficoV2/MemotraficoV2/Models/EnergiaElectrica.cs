using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class EnergiaElectrica
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.EnergiaElectrica.AddObject(this);
            db.SaveChanges();
        }

        public void Editar()
        {
            SASEntities db = new SASEntities();
            EnergiaElectrica ee = db.EnergiaElectrica.FirstOrDefault(i => i.IdValidarFk == IdValidarFk);
            ee.Subestacion = Subestacion;
            ee.SubestacionEstado = SubestacionEstado;
            ee.MuroAcometida = MuroAcometida;
            ee.MuroAcometidaEstado = MuroAcometidaEstado;
            ee.TableroDistribucion = TableroDistribucion;
            ee.TableroDistribucionEstado = TableroDistribucionEstado;
            ee.InterruptorGral = InterruptorGral;
            ee.InterruptorGralEstado = InterruptorGralEstado;
            ee.Termomagnetico = Termomagnetico;
            ee.TermomagneticoElectrico = TermomagneticoElectrico;
            ee.AlumbradoExt = AlumbradoExt;
            ee.AlumbradoExtEstado = AlumbradoExtEstado;
            ee.Luminaria = Luminaria;
            ee.LuminariaEstado = LuminariaEstado;
            ee.EstadoGeneral = EstadoGeneral;
            db.SaveChanges();
        }

        public static EnergiaElectrica ObtenerRegistro(int valor)
        {
            SASEntities db = new SASEntities();
            EnergiaElectrica ee = new EnergiaElectrica();
            var registro = db.EnergiaElectrica.Where(i => i.IdValidarFk == valor).Count() > 0 ? false : true;
            if (registro)
            {
                ee.IdValidarFk = valor;
                ee.Subestacion = false;
                ee.SubestacionEstado = 0;
                ee.MuroAcometida = false;
                ee.MuroAcometidaEstado = 0;
                ee.TableroDistribucion = false;
                ee.TableroDistribucionEstado = 0;
                ee.InterruptorGral = false;
                ee.InterruptorGralEstado = 0;
                ee.Termomagnetico = false;
                ee.TermomagneticoElectrico = 0;
                ee.AlumbradoExt = false;
                ee.AlumbradoExtEstado = 0;
                ee.Luminaria = false;
                ee.LuminariaEstado = 0;
                ee.EstadoGeneral = 0;
                ee.Crear();
            }
            ee = db.EnergiaElectrica.FirstOrDefault(i => i.IdValidarFk == valor);
            return ee;
        }
    }
}