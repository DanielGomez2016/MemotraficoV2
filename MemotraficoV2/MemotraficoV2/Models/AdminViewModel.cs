using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Rol")]
        public string Name { get; set; }
        public string Descripcion { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }

        [Required]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }

        [Required]
        [Display(Name = "Departamento")]
        public int IdDepartamento { get; set; }

        [Required]
        [Display(Name = "Institucion")]
        public int IdInstitucion { get; set; }
    }
}