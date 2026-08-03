using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGimn.Models
{
    public class Membresia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del plan es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Plan de Membresía")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio ($)")]
        public decimal Precio { get; set; }

        [Required]
        [Display(Name = "Duración (Días)")]
        public int DuracionDias { get; set; } = 30;

        [StringLength(500)]
        [Display(Name = "Descripción del Plan")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Plan Activo")]
        public bool EstaActivo { get; set; } = true;
    }
}
