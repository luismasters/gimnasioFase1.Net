using System.ComponentModel.DataAnnotations;

namespace AppGimn.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordar mis datos")]
        public bool RememberMe { get; set; }

        // Para redirigir después del login
        public string? ReturnUrl { get; set; }
    }
}