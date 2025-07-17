using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGymFront.Models
{
    public class ErroresReportados
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El {0} no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Correo { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string Celular { get; set; } = string.Empty;

        [Display(Name = "Asunto")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Asunto { get; set; }

        [Display(Name = "Mensaje")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Mensaje { get; set; }

        public DateTime FechaReporte { get; set; } = DateTime.Now;
    }
}