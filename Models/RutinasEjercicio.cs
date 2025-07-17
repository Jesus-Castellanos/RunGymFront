using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGymFront.Models
{
    public class RutinasEjercicio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo IdUsuario es obligatorio.")]
        public int IdUsuario { get; set; }


        [Required(ErrorMessage = "El campo NombreRutina es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la rutina no puede exceder los 100 caracteres.")]
        public string NombreRutina { get; set; }


        [Required(ErrorMessage = "El campo FechaInicio es obligatorio.")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }


        [Required(ErrorMessage = "El campo FechaFin es obligatorio.")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }


        [Range(1, 100, ErrorMessage = "La duración de la rutina debe estar entre 1 y 100 días.")]
        public int DuracionDias { get; set; }
    }
}