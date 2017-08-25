using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Beneficiario
    {
        public int Crear()
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Beneficiario.AddObject(this);
                db.SaveChanges();

                return IdBeneficiario;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Editar()
        {
            try
            {
                SASEntities db = new SASEntities();
                Beneficiario e = db.Beneficiario.FirstOrDefault(i => i.IdBeneficiario == this.IdBeneficiario);
                e.Nombre = Nombre;
                e.Domicilio = Domicilio;
                e.IdLocalidadFk = IdLocalidadFk;
                e.IdMunicipioFk = IdMunicipioFk;
                e.Telefono = Telefono;
                e.Celular = Celular;
                e.Email = Email;

                db.SaveChanges();

                return IdBeneficiario;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}