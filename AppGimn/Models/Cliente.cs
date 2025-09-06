using System.ComponentModel.DataAnnotations;

namespace AppGimn.Models
{
    public class Cliente
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

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Fecha de Inscripción")]
        [DataType(DataType.Date)]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [StringLength(100, ErrorMessage = "El contacto de emergencia no puede tener más de 100 caracteres")]
        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones médicas no pueden tener más de 500 caracteres")]
        [Display(Name = "Observaciones Médicas")]
        [DataType(DataType.MultilineText)]
        public string? ObservacionesMedicas { get; set; }

        [Display(Name = "¿Está Activo?")]
        public bool EstaActivo { get; set; } = true;

        // ============ PROPERTIES CALCULADAS ============
        // Esto demuestra tu dominio de C# moderno

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombre} {Apellido}";

        [Display(Name = "Edad")]
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;

                // Ajustar si aún no cumplió años este año
                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;

                return edad;
            }
        }

        // Útil para validaciones de negocio
        public bool EsMayorDeEdad => Edad >= 18;

        // Para ordenamientos y filtros
        public string ApellidoNombre => $"{Apellido}, {Nombre}";

    
    }
}