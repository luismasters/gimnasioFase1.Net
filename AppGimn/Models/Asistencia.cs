using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGimn.Models
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        [Display(Name = "Fecha y Hora de Ingreso")]
        public DateTime FechaHoraIngreso { get; set; } = DateTime.Now;

        [Display(Name = "Fecha y Hora de Salida")]
        public DateTime? FechaHoraSalida { get; set; }

        [Display(Name = "Acceso Permitido")]
        public bool Permitido { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Motivo de Observación / Denegado")]
        public string? MotivoDenegado { get; set; }
    }
}
