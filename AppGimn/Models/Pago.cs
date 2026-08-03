using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGimn.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        public int? MembresiaId { get; set; }

        [ForeignKey("MembresiaId")]
        public virtual Membresia? Membresia { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto Cobrado ($)")]
        public decimal Monto { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Vencimiento Habilitada")]
        public DateTime FechaVencimiento { get; set; } = DateTime.Now.AddDays(30);

        [Required]
        [StringLength(50)]
        [Display(Name = "Medio de Pago")]
        public string MedioPago { get; set; } = "Efectivo";

        [StringLength(50)]
        [Display(Name = "Comprobante N°")]
        public string ComprobanteNumero { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Cobrado Por (Operador)")]
        public string RecepcionistaEmail { get; set; } = string.Empty;
    }
}
