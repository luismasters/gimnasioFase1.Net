using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<Usuario> _userManager;

        public BaseController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ OBTENER USUARIO ACTUAL
        protected async Task<Usuario?> ObtenerUsuarioActual()
        {
            return await _userManager.GetUserAsync(User);
        }

        // ✅ VERIFICAR PERMISOS PARA GESTIONAR CLIENTES
        protected async Task<bool> PuedeGestionarClientes()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario == null) return false;

            if (usuario.EsAdmin) return true;
            if (!usuario.EsEmpleado) return false;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI);

            if (empleado == null) return false;

            return empleado.Cargo.Equals("Gerente", StringComparison.OrdinalIgnoreCase) ||
                   empleado.Cargo.Equals("Recepcionista", StringComparison.OrdinalIgnoreCase);
        }

        // ✅ VERIFICAR PERMISOS PARA GESTIONAR EMPLEADOS
        protected async Task<bool> PuedeGestionarEmpleados()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario == null) return false;

            if (usuario.EsAdmin) return true;
            if (!usuario.EsEmpleado) return false;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI);

            if (empleado == null) return false;

            return empleado.Cargo.Equals("Gerente", StringComparison.OrdinalIgnoreCase);
        }

        // ✅ VERIFICAR SI TIENE ACCESO AL DASHBOARD
        protected async Task<bool> TieneAccesoDashboard()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario == null) return false;

            return usuario.EsAdmin || usuario.EsEmpleado;
        }

        // ✅ MÉTODO PARA REDIRECCIONAR CON MENSAJE DE ERROR
        protected IActionResult RedirectConError(string mensaje, string controller = "Home", string action = "Index")
        {
            TempData["Error"] = mensaje;
            return RedirectToAction(action, controller);
        }

        // ✅ MÉTODO PARA REDIRECCIONAR CON MENSAJE DE ÉXITO
        protected IActionResult RedirectConExito(string mensaje, string controller = "Home", string action = "Index")
        {
            TempData["Success"] = mensaje;
            return RedirectToAction(action, controller);
        }
    }
}