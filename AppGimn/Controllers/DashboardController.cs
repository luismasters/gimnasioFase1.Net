using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context, UserManager<Usuario> userManager)
            : base(context, userManager)
        {
        }

        // ============ RUTA 5 - ADMIN (DASHBOARD EJECUTIVO) ============
        public async Task<IActionResult> Index()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario == null) return RedirectToAction("Login", "Account");

            // Redirección estricta según el rol del usuario autenticado
            if (usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }
            if (usuario.EsEmpleado && !usuario.EsAdmin)
            {
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.DNI == usuario.DNI || e.Email == usuario.Email);
                if (empleado != null && empleado.Cargo.Equals("Recepcionista", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("RecepcionPanel");
                }
                if (empleado != null && empleado.Cargo.Equals("Entrenador", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("InstructorPanel");
                }
            }

            ViewBag.Usuario = usuario;

            var dashboardData = new DashboardViewModel
            {
                TotalClientes = await _context.Clientes.CountAsync(c => c.EstaActivo),
                TotalEmpleados = await _context.Empleados.CountAsync(e => e.EstaActivo),
                ClientesInactivos = await _context.Clientes.CountAsync(c => !c.EstaActivo),
                EmpleadosInactivos = await _context.Empleados.CountAsync(e => !e.EstaActivo),
                ClientesRecientes = await _context.Clientes
                    .Where(c => c.FechaInscripcion >= DateTime.Now.AddDays(-7))
                    .CountAsync(),
                EmpleadosPorCargo = await _context.Empleados
                    .Where(e => e.EstaActivo)
                    .GroupBy(e => e.Cargo)
                    .Select(g => new CargoCantidad { Cargo = g.Key, Cantidad = g.Count() })
                    .ToListAsync()
            };

            ViewBag.PuedeGestionarClientes = await PuedeGestionarClientes();
            ViewBag.PuedeGestionarEmpleados = await PuedeGestionarEmpleados();

            return View(dashboardData);
        }

        // ============ RUTA 2 - CLIENTE (MI GIMNASIO PERSONAL - DASHBOARD SOCIO) ============
        public async Task<IActionResult> ClientePanel()
        {
            var usuario = await ObtenerUsuarioActual();
            var cliente = usuario != null 
                ? await _context.Clientes.FirstOrDefaultAsync(c => c.Email == usuario.Email || c.DNI == usuario.DNI) 
                : null;

            ViewBag.ClienteNombre = cliente != null ? cliente.NombreCompleto : "Carlos Gómez";
            ViewBag.MiembroDesde = cliente != null ? cliente.FechaInscripcion.ToString("MMMM yyyy") : "Marzo 2024";
            ViewBag.MembresiaNombre = "Pase Premium Aura 24/7";
            ViewBag.ProximaClase = "Pilates Reformer - Hoy 18:00 hs";
            ViewBag.EntrenadorAsignado = "Marcus Vance (Head Coach)";

            return View();
        }

        public async Task<IActionResult> ClienteRutinas()
        {
            var usuario = await ObtenerUsuarioActual();
            var cliente = usuario != null 
                ? await _context.Clientes.FirstOrDefaultAsync(c => c.Email == usuario.Email || c.DNI == usuario.DNI) 
                : null;

            ViewBag.ClienteNombre = cliente != null ? cliente.NombreCompleto : "Carlos Gómez";
            return View();
        }

        public async Task<IActionResult> ClienteEvolucion()
        {
            var usuario = await ObtenerUsuarioActual();
            var cliente = usuario != null 
                ? await _context.Clientes.FirstOrDefaultAsync(c => c.Email == usuario.Email || c.DNI == usuario.DNI) 
                : null;

            ViewBag.ClienteNombre = cliente != null ? cliente.NombreCompleto : "Carlos Gómez";
            return View();
        }

        public async Task<IActionResult> ClienteClases()
        {
            await Task.CompletedTask;
            return View();
        }

        // ============ RUTA 3 - RECEPCIONISTA (ESTOY TRABAJANDO - ALTA DENSIDAD) ============
        public async Task<IActionResult> RecepcionPanel()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario == null) return RedirectToAction("Login", "Account");

            // Si es un cliente y no admin/empleado, redirigir a su panel
            if (usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var clientesPresentes = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();

            ViewBag.ClientesPresentesCount = 14;
            ViewBag.VencimientosCount = 5;
            ViewBag.CajaDiaTotal = "$ 145.000";

            return View(clientesPresentes);
        }

        public async Task<IActionResult> RecepcionCobros()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var clientes = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();
            return View(clientes);
        }

        // ============ RUTA 4 - INSTRUCTOR (ALUMNOS & AGENDA DE ENTRENAMIENTO) ============
        public async Task<IActionResult> InstructorPanel()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var misAlumnos = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();
            ViewBag.ClasesHoyCount = 4;
            ViewBag.ProximaClaseHora = "17:30 hs - Musculación & Biomecánica";

            return View(misAlumnos);
        }

        // ============ ESTADÍSTICAS GLOBALES ============
        public async Task<IActionResult> Estadisticas()
        {
            var estadisticas = new EstadisticasViewModel
            {
                ClientesPorMes = await _context.Clientes
                    .Where(c => c.FechaInscripcion >= DateTime.Now.AddMonths(-6))
                    .GroupBy(c => new { c.FechaInscripcion.Year, c.FechaInscripcion.Month })
                    .Select(g => new MesConteo
                    {
                        Año = g.Key.Year,
                        Mes = g.Key.Month,
                        Cantidad = g.Count()
                    })
                    .OrderBy(x => x.Año).ThenBy(x => x.Mes)
                    .ToListAsync(),

                EmpleadosPorAntiguedad = await _context.Empleados
                    .Where(e => e.EstaActivo)
                    .Select(e => new EmpleadoAntiguedad
                    {
                        NombreCompleto = e.NombreCompleto,
                        Cargo = e.Cargo,
                        AntiguedadDias = e.AntiguedadDias
                    })
                    .OrderByDescending(e => e.AntiguedadDias)
                    .Take(10)
                    .ToListAsync()
            };

            return View(estadisticas);
        }
    }
}