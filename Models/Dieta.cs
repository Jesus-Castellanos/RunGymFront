using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RunGymFront.Models
{
    public class Dieta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required(ErrorMessage = "Los macronutrientes son obligatorios.")]
        [StringLength(500, ErrorMessage = "Los macronutrientes no pueden exceder los 100 caracteres.")]
        public string Desayuno { get; set; }


        [Required(ErrorMessage = "El tipo de dieta es obligatorio.")]
        [StringLength(500, ErrorMessage = "El tipo de dieta no puede exceder los 50 caracteres.")]
        public string Almuerzo { get; set; }

        [Required(ErrorMessage = "Los macronutrientes son obligatorios.")]
        [StringLength(500, ErrorMessage = "Los macronutrientes no pueden exceder los 100 caracteres.")]
        public string Cena { get; set; }

        [Required(ErrorMessage = "Los macronutrientes son obligatorios.")]
        [StringLength(500, ErrorMessage = "Los macronutrientes no pueden exceder los 100 caracteres.")]
        public string Snacks { get; set; }
        [JsonIgnore]
        public int TipoDietaId { get; set; }


        [ForeignKey("IdTipoDieta")]
        public virtual TipoDieta? TipoDieta { get; set; }
    }
}