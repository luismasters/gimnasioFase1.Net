using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario>
    {
        private readonly ApplicationDbContext _context;

        public CustomClaimsPrincipalFactory(
            UserManager<Usuario> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            ApplicationDbContext context)
            : base(userManager, optionsAccessor)
        {
            _context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            try
            {
                // Claims básicos del usuario
                identity.AddClaim(new Claim("EsAdmin", user.EsAdmin.ToString()));
                identity.AddClaim(new Claim("EsEmpleado", user.EsEmpleado.ToString()));
                identity.AddClaim(new Claim("EsCliente", user.EsCliente.ToString()));

                // Agregar DNI como claim
                if (!string.IsNullOrEmpty(user.DNI))
                {
                    identity.AddClaim(new Claim("DNI", user.DNI));
                }

                // Si es empleado, agregar claims específicos
                if (user.EsEmpleado && !string.IsNullOrEmpty(user.DNI))
                {
                    var empleado = await _context.Empleados
                        .FirstOrDefaultAsync(e => e.DNI == user.DNI && e.EstaActivo);

                    if (empleado != null)
                    {
                        identity.AddClaim(new Claim("Cargo", empleado.Cargo));
                        identity.AddClaim(new Claim("PuedeGestionarClientes", empleado.PuedeGestionarClientes.ToString()));
                        identity.AddClaim(new Claim("PuedeGestionarEmpleados", empleado.PuedeGestionarEmpleados.ToString()));
                        identity.AddClaim(new Claim("PuedeVerReportes", empleado.PuedeVerReportes.ToString()));
                    }
                }

                // Si es cliente, agregar información específica
                if (user.EsCliente && !string.IsNullOrEmpty(user.DNI))
                {
                    var cliente = await _context.Clientes
                        .FirstOrDefaultAsync(c => c.DNI == user.DNI && c.EstaActivo);

                    if (cliente != null)
                    {
                        identity.AddClaim(new Claim("ClienteId", cliente.Id.ToString()));
                        identity.AddClaim(new Claim("FechaInscripcion", cliente.FechaInscripcion.ToString("yyyy-MM-dd")));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error - en producción usarías ILogger
                System.Diagnostics.Debug.WriteLine($"Error generando claims para usuario {user.Email}: {ex.Message}");

                // Asegurar que al menos los claims básicos estén presentes
                if (!identity.Claims.Any(c => c.Type == "EsAdmin"))
                    identity.AddClaim(new Claim("EsAdmin", user.EsAdmin.ToString()));
                if (!identity.Claims.Any(c => c.Type == "EsEmpleado"))
                    identity.AddClaim(new Claim("EsEmpleado", user.EsEmpleado.ToString()));
                if (!identity.Claims.Any(c => c.Type == "EsCliente"))
                    identity.AddClaim(new Claim("EsCliente", user.EsCliente.ToString()));
            }

            return identity;
        }
    }
}