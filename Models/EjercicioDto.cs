using System.Collections.Generic;
namespace RunGymFront.Models
{
    public class EjercicioDto
    {
        public string Nombre { get; set; }
        public List<DetalleDto> Detalles { get; set; }
        public List<PasoDto> Pasos { get; set; }
    }

    public class DetalleDto
    {
        public string Descripcion { get; set; }
        public string Repeticiones { get; set; }
        public string Descanso { get; set; }
        public string Cuidados { get; set; }
        public string URLVideo { get; set; }
    }

    public class PasoDto
    {
        public int NumeroPaso { get; set; }
        public string DescripcionPaso { get; set; }
    }
}