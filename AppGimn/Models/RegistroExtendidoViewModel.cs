using System.ComponentModel.DataAnnotations;

namespace AppGimn.Models
{
    public class RegistroExtendidoViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ============ CAMPOS ADICIONALES ============
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(10, ErrorMessage = "El DNI no puede tener más de 10 caracteres")]
        [Display(Name = "DNI")]
        public string DNI { get; set; } = string.Empty;

        [Display(Name = "¿Soy empleado del gimnasio?")]
        public bool EsEmpleado { get; set; } = false;

        [Display(Name = "¿Soy cliente del gimnasio?")]
        public bool EsCliente { get; set; } = false;

        // Solo admins pueden crear otros admins - esto se maneja en el controller
        public bool EsAdmin { get; set; } = false;
    }
}