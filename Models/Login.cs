using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunGymFront.Models
{
    public class Login
    {
        [DisplayName("Correo")]
        [Required(ErrorMessage = "El correo {0} es requerido.")]
        [EmailAddress]
        public string Correo { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La {0} es requerido.")]
        public string Contraseña { get; set; }
    }
}