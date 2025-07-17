using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunGymFront.Models
{
    public class Login
    {
        [DisplayName("Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress]
        public string Correo { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Contraseña { get; set; }
    }
}