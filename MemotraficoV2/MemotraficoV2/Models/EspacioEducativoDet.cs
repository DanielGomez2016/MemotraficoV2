using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class EspacioEducativoDet
    {
        //cuenta las aulas que se tienen registradas en la validacion
        public static int ContarAulas(int val)
        {
            SASEntities db = new SASEntities();
            List<EspacioEducativoDet> ed = new List<EspacioEducativoDet>();
            int aulas = 0;
            var edu = db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == val);
            if (edu != null) {
                ed = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == edu.IdEspacioEducativo).ToList();
                
                foreach (var i in ed)
                {
                    aulas += aulas + Convert.ToInt32(i.Aulas);
                }
            }

            return aulas;
        }

        //cuenta los laboratorios que se tienen registrados en la validacion
        public static int ContarLaboratorios(int val)
        {
            SASEntities db = new SASEntities();
            List<EspacioEducativoDet> ed = new List<EspacioEducativoDet>();
            int labs = 0;
            var edu = db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == val);
            if (edu != null)
            {
                ed = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == edu.IdEspacioEducativo).ToList();
                
                foreach (var i in ed)
                {
                    labs += labs + Convert.ToInt32(i.Laboratirio);
                }
            }

            return labs;
        }

        //cuenta los talleres que se tienen registrados en la validacion
        public static int ContarTalleres(int val)
        {
            SASEntities db = new SASEntities();
            List<EspacioEducativoDet> ed = new List<EspacioEducativoDet>();
            int taller = 0;
            var edu = db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == val);
            if (edu != null)
            {
                ed = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == edu.IdEspacioEducativo).ToList();
                
                foreach (var i in ed)
                {
                    taller += taller + Convert.ToInt32(i.Taller);
                }
            }

            return taller;
        }

        //cuenta los anexos que se tienen registrados en la validacion
        public static int ContarAnexos(int val)
        {
            SASEntities db = new SASEntities();
            List<EspacioEducativoDet> ed = new List<EspacioEducativoDet>();
            int anexos = 0;
            var edu = db.EspacioEducativo.FirstOrDefault(j => j.IdValidarFk == val);
            if (edu != null)
            {
                ed = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == edu.IdEspacioEducativo).ToList();
                foreach (var i in ed)
                {
                    anexos += anexos +
                        Convert.ToInt32(i.ServicioAdministrativoBiblio) +
                        Convert.ToInt32(i.ServicioSanitario) +
                        Convert.ToInt32(i.AulasUsoMultiple) +
                        Convert.ToInt32(i.BodegaApendice);
                }
            }

            return anexos;
        }

        public void Crear()
        {
            SASEntities db = new SASEntities();
            db.EspacioEducativoDet.AddObject(this);
            db.SaveChanges();
        }

        public static void EliminaRegistros(int valor)
        {
            try
            {
                SASEntities db = new SASEntities();
                List<EspacioEducativoDet> edd = db.EspacioEducativoDet.Where(i => i.IdEspacioEducativoFk == valor).ToList();

                foreach (var i in edd)
                {
                    db.EspacioEducativoDet.DeleteObject(i);
                    db.SaveChanges();
                }
            }
            catch (Exception e) { }

        }
    }
}