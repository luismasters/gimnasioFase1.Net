using System.Security.Claims;
using AppGimn.Data;
using Microsoft.EntityFrameworkCore;

namespace AppGimn.Helpers
{
    public class PermissionHelper
    {
        private readonly ApplicationDbContext _context;

        public PermissionHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public static bool IsAdmin(ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == "EsAdmin" && c.Value == "True");
        }

        public static bool IsEmpleado(ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == "EsEmpleado" && c.Value == "True");
        }

        public static bool IsCliente(ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == "EsCliente" && c.Value == "True");
        }

        public static bool CanManageClientes(ClaimsPrincipal user)
        {
            return IsAdmin(user) || user.Claims.Any(c => c.Type == "PuedeGestionarClientes" && c.Value == "True");
        }

        public static bool CanManageEmpleados(ClaimsPrincipal user)
        {
            return IsAdmin(user) || user.Claims.Any(c => c.Type == "PuedeGestionarEmpleados" && c.Value == "True");
        }

        public static bool CanViewReports(ClaimsPrincipal user)
        {
            return IsAdmin(user) || user.Claims.Any(c => c.Type == "PuedeVerReportes" && c.Value == "True");
        }

        public static string GetUserDNI(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "DNI")?.Value ?? "";
        }

        public static string GetUserCargo(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "Cargo")?.Value ?? "";
        }

        public static string GetUserType(ClaimsPrincipal user)
        {
            if (IsAdmin(user)) return "Admin";
            if (IsEmpleado(user)) return "Empleado";
            if (IsCliente(user)) return "Cliente";
            return "Usuario";
        }

        public async Task<bool> UserCanAccessEmpleado(ClaimsPrincipal user, int empleadoId)
        {
            // Admin puede acceder a cualquier empleado
            if (IsAdmin(user)) return true;

            // Un empleado puede acceder a su propio perfil
            if (IsEmpleado(user))
            {
                var userDNI = GetUserDNI(user);
                if (!string.IsNullOrEmpty(userDNI))
                {
                    var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);
                    if (empleado != null && empleado.DNI == userDNI)
                        return true;
                }

                // Empleados con permisos pueden ver otros empleados
                return CanManageEmpleados(user);
            }

            return false;
        }

        public async Task<bool> UserCanAccessCliente(ClaimsPrincipal user, int clienteId)
        {
            // Admin puede acceder a cualquier cliente
            if (IsAdmin(user)) return true;

            // Empleados con permisos pueden acceder a clientes
            if (IsEmpleado(user) && CanManageClientes(user))
                return true;

            // Un cliente puede acceder a su propio perfil
            if (IsCliente(user))
            {
                var userDNI = GetUserDNI(user);
                if (!string.IsNullOrEmpty(userDNI))
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);
                    return cliente != null && cliente.DNI == userDNI;
                }
            }

            return false;
        }

        public static Dictionary<string, bool> GetAllPermissions(ClaimsPrincipal user)
        {
            return new Dictionary<string, bool>
            {
                ["IsAdmin"] = IsAdmin(user),
                ["IsEmpleado"] = IsEmpleado(user),
                ["IsCliente"] = IsCliente(user),
                ["CanManageClientes"] = CanManageClientes(user),
                ["CanManageEmpleados"] = CanManageEmpleados(user),
                ["CanViewReports"] = CanViewReports(user)
            };
        }

        public static bool HasAnyPermission(ClaimsPrincipal user)
        {
            return user.Identity?.IsAuthenticated == true &&
                   (IsAdmin(user) || IsEmpleado(user) || IsCliente(user));
        }

        public static string GetPermissionDescription(ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return "No autenticado";

            var permissions = new List<string>();

            if (IsAdmin(user))
                permissions.Add("Administrador");

            if (IsEmpleado(user))
            {
                var cargo = GetUserCargo(user);
                permissions.Add($"Empleado{(!string.IsNullOrEmpty(cargo) ? $" ({cargo})" : "")}");

                if (CanManageClientes(user)) permissions.Add("Gestión de Clientes");
                if (CanManageEmpleados(user)) permissions.Add("Gestión de Empleados");
                if (CanViewReports(user)) permissions.Add("Reportes");
            }

            if (IsCliente(user))
                permissions.Add("Cliente");

            return permissions.Count > 0 ? string.Join(", ", permissions) : "Sin permisos específicos";
        }
    }
}