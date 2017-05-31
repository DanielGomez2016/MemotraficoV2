﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Validacion
    {
        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.Validacion.AddObject(this);
            db.SaveChanges();
        }

        public static Validacion ObtenerRegistro(int escuela)
        {
            SASEntities db = new SASEntities();
            Validacion v = new Validacion();
            var registro = db.Validacion.Where(i => i.IdEscuelaFk == escuela).Count() > 0 ? false : true;
            if (registro)
            {
                v.IdEscuelaFk = escuela;
                v.Crear();
            }
            v = db.Validacion.FirstOrDefault(i => i.IdEscuelaFk == escuela);
            return v;
        }

        public int CountRegistro(int esc)
        {
            SASEntities db = new SASEntities();
            var valor = db.Validacion.Where(i => i.IdEscuelaFk == esc).Count();
            return valor;
        }
    }
}