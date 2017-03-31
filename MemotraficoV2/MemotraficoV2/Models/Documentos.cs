using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public partial class Documentos
    {
        public void CrearDoc()
        {
            SASEntities db = new SASEntities();
            db.Documentos.AddObject(this);
            db.SaveChanges();
        }
    }
}