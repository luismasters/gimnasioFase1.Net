using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppGimn.Models
{
    public class Usuario : IdentityUser
    {
        // ============ DATOS PARA VINCULAR CON ENTIDADES DE NEGOCIO ============

        [StringLength(10, ErrorMessage = "El DNI no puede tener más de 10 caracteres")]
        [Display(Name = "DNI")]
        public string? DNI { get; set; }

        // ============ ROLES DEL SISTEMA ============
        // Usamos bool para simplificar (después se puede migrar a Roles de Identity)

        [Display(Name = "¿Es Cliente?")]
        public bool EsCliente { get; set; } = false;

        [Display(Name = "¿Es Empleado?")]
        public bool EsEmpleado { get; set; } = false;

        [Display(Name = "¿Es Administrador?")]
        public bool EsAdmin { get; set; } = false;
     
    }
}