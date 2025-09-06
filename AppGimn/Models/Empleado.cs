using System.ComponentModel.DataAnnotations;

namespace AppGimn.Models
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(10, ErrorMessage = "El DNI no puede tener más de 10 caracteres")]
        [Display(Name = "DNI")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(30, ErrorMessage = "El cargo no puede tener más de 30 caracteres")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser un valor positivo")]
        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        public decimal? Salario { get; set; }

        [Display(Name = "¿Está Activo?")]
        public bool EstaActivo { get; set; } = true;

        [StringLength(200, ErrorMessage = "Las observaciones no pueden tener más de 200 caracteres")]
        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string? Observaciones { get; set; }

        // Properties calculadas
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public int AntiguedadDias => (DateTime.Now - FechaIngreso).Days;

        // Lógica de permisos por cargo
        public bool PuedeGestionarClientes => Cargo == "Recepcionista" || Cargo == "Gerente";
        public bool PuedeGestionarEmpleados => Cargo == "Gerente";
        public bool PuedeVerReportes => Cargo == "Gerente";

    }
}