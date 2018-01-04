using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MemotraficoV2.Models.Colecciones;
using MemotraficoV2.ViewModels;

namespace MemotraficoV2.Models
{
    [MetadataType(typeof(mEscuela))]
    public partial class Escuela
    {
        public class mEscuela
        {
            [Required]
            [Display(Name ="Plantel Educativo")]
            public string Nombre { get; set; }

            [Display(Name = "Municipio")]
            public string IdMunicipioFk { get; set; }

            [Display(Name = "Localidad")]
            public string IdLocalidadFk { get; set; }

            [Display(Name = "Nivel Educativo")]
            public string IdNivelEducativo { get; set; }

            [Display(Name = "Geolocalizacion X")]
            public string Geox { get; set; }

            [Display(Name = "Geolocalizacion Y")]
            public string Geoy { get; set; }
        }

        [Display(Name = "Nombre Director")]
        public string NombreDirector { get; set; }

        [Display(Name = "Email Director")]
        public string EmailDirector { get; set; }

        public string Telefono { get; set; }
        public string Celular { get; set; }

        public int Crear()
        {
            try
            {
                SASEntities db = new SASEntities();
                db.Escuela.AddObject(this);
                db.SaveChanges();

                return IdEscuela;

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
                Escuela e = db.Escuela.FirstOrDefault(i => i.IdEscuela == this.IdEscuela);
                e.Nombre = Nombre != null ? Nombre : e.Nombre;
                e.Clave = Clave != null ? Clave : e.Clave;
                e.Domicilio = Domicilio != null ? Domicilio : e.Domicilio;
                e.IdLocalidadFk = IdLocalidadFk != null ? IdLocalidadFk : e.IdLocalidadFk;
                e.IdMunicipioFk = IdMunicipioFk != null ? IdMunicipioFk : e.IdMunicipioFk;
                e.IdNivelEducativo = IdNivelEducativo != null ? IdNivelEducativo : e.IdNivelEducativo;
                e.Turno = Turno != null ? Turno : e.Turno;
                e.Geox = Geox != null ? Geox : e.Geox;
                e.Geoy = Geoy != null ? Geoy : e.Geoy;

                db.SaveChanges();

                return IdEscuela;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static object AllEsceulas(AnexGRID grid)
        {
            grid.Inicializar();

            try
            {
                SASEntities db = new SASEntities();

                var e = db.Escuela.AsQueryable();

                if (grid.columna == "memotrafico")
                {
                    e = grid.columna_orden == "DESC" ? e.OrderBy(x => x.IdEscuela)
                                                        : e.OrderByDescending(x => x.IdEscuela);
                }

                foreach (var f in grid.filtros)
                {
                    if (f.columna == "clave")
                        e = e.Where(x => x.Clave.Contains(f.valor));
                    if (f.columna == "nombre")
                        e = e.Where(x => x.Nombre.Contains(f.valor));
                    if (f.columna == "localidad")
                        e = e.Where(x => x.Localidades.Nombre.Contains(f.valor) || x.Municipios.Nombre.Contains(f.valor));
                }

                var query = e.Select(x => new escuelas
                {
                    id = x.IdEscuela,
                    nombre = x.Nombre,
                    clave = x.Clave,
                    nivel = x.NivelEducativo.Nivel,
                    localidad = x.Localidades.Nombre + ", " + x.Municipios.Nombre,
                    director = x.Contacto.FirstOrDefault(y => y.IdEscuelaFk == x.IdEscuela).Nombre,
                    telefono = x.Contacto.FirstOrDefault(y => y.IdEscuelaFk == x.IdEscuela).Telefono,
                    email = x.Contacto.FirstOrDefault(y => y.IdEscuelaFk == x.IdEscuela).Email,
                }).ToList();

                var data = query
                           .Skip(grid.pagina)
                           .Take(grid.limite)
                           .ToList();

                var total = query.Count();

                grid.SetData(data, total);
            }
            catch (Exception e)
            {

            }

            return grid.responde();
        }

        public int EditarNivel()
        {
            try
            {
                SASEntities db = new SASEntities();
                Escuela e = db.Escuela.FirstOrDefault(i => i.Clave == this.Clave);
                e.IdNivelEducativo = IdNivelEducativo != null ? IdNivelEducativo : e.IdNivelEducativo;
                db.SaveChanges();

                return IdEscuela;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }


}