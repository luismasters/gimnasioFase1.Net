using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGimn.Models
{
    public class EvaluacionFisica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        [Display(Name = "Fecha de Evaluación")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [Display(Name = "Peso (kg)")]
        public double PesoKg { get; set; }

        [Display(Name = "% Grasa Corporal")]
        public double PorcentajeGrasa { get; set; }

        [Display(Name = "Masa Muscular (kg)")]
        public double MasaMuscularKg { get; set; }

        [Display(Name = "Perímetro Tórax (cm)")]
        public double ToraxCm { get; set; }

        [Display(Name = "Perímetro Cintura (cm)")]
        public double CinturaCm { get; set; }

        [Display(Name = "Perímetro Bíceps (cm)")]
        public double BicepsCm { get; set; }

        [StringLength(500)]
        [Display(Name = "Observaciones del Entrenador")]
        public string? Observaciones { get; set; }
    }
}
