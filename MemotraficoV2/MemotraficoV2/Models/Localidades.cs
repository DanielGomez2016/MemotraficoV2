﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mLocalidades))]
    public partial class Localidades
    {

        public class mLocalidades
        {
            [Required]
            [Display(Name = "Municipio")]
            public string IdMunicipioFk { get; set; }
        }

        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.Localidades.AddObject(this);
            db.SaveChanges();
        }
        public void Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Localidades c = db.Localidades.FirstOrDefault(i => i.IdLocalidad == this.IdLocalidad);
                c.Nombre = Nombre;
                c.IdMunicipioFk = IdMunicipioFk;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static int IdLocalidades(string claveloc, int m)
        {

            var str = Convert.ToInt32(claveloc);

            SASEntities db = new SASEntities();
            return db.Localidades.FirstOrDefault(i => i.ClaveLocalidad.Contains(str.ToString()) && i.IdMunicipioFk == m).IdLocalidad;
        }
    }
}