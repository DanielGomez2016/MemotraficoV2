using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class NivelEducativo
    {

        public static int IdNivelEdu(string nivel)
        {
            SASEntities db = new SASEntities();
            return db.NivelEducativo.FirstOrDefault(i => i.Nivel.Contains(nivel)).IdNivelEducativo;
        }
    }
}