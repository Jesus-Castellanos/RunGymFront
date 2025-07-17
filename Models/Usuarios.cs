using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGymFront.Models
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El {0} no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El {0} no puede exceder 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Tipo de Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20, ErrorMessage = "El {0} no puede exceder 20 caracteres")]
        public string Documento { get; set; } = string.Empty;

        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Correo { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La {0} debe tener al menos 4 caracteres.")]
        public string Contraseña { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string Celular { get; set; } = string.Empty;

        [Display(Name = "Género")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Genero { get; set; } = string.Empty;

        [Display(Name = "Fecha de Nacimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; } = DateTime.UtcNow;

        [Display(Name = "Peso (kg)")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 200, ErrorMessage = "El {0} debe estar entre 1 y 200 kg")]
        public int Peso { get; set; }

        [Display(Name = "Altura")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1.10, 2.50, ErrorMessage = "La {0} debe estar entre 1.10 y 2.50 metros")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public Decimal Altura { get; set; }

        [Display(Name = "Horas de Sueño")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(4, 12, ErrorMessage = "Las {0} deben estar entre 4 y 12 horas")]
        public byte HorasSueno { get; set; }

        [Display(Name = "Consumo de Agua (litros)")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ConsumoAgua { get; set; } = string.Empty;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;



        public string? CodigoVerificacion { get; set; }
        public DateTime? CodigoExpira { get; set; }

        public int RolId { get; set; }

        public virtual Rol Rol { get; set; }
    }
}
