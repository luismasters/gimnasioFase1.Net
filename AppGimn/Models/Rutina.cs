using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGimn.Models
{
    public class Rutina
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        public int? InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Empleado? Instructor { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Nombre de la Rutina")]
        public string Nombre { get; set; } = "Rutina de Entrenamiento";

        [Display(Name = "Fecha de Asignación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Día / Frecuencia")]
        public string DiaSemana { get; set; } = "Lunes: Pecho & Tríceps";

        [Display(Name = "Rutina Activa")]
        public bool EstaActiva { get; set; } = true;

        public virtual ICollection<EjercicioRutina> Ejercicios { get; set; } = new List<EjercicioRutina>();
    }

    public class EjercicioRutina
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RutinaId { get; set; }

        [ForeignKey("RutinaId")]
        public virtual Rutina? Rutina { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Nombre del Ejercicio")]
        public string NombreEjercicio { get; set; } = string.Empty;

        [Required]
        public int Series { get; set; } = 4;

        [Required]
        [StringLength(50)]
        public string Repeticiones { get; set; } = "10 - 12";

        [StringLength(50)]
        public string CargaSugerida { get; set; } = "60 kg";

        public int DescansoSegundos { get; set; } = 90;

        [StringLength(250)]
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
