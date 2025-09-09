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

        public async Task<IActionResult> Index()
        {
            // Verificar si tiene acceso al dashboard
            if (!await TieneAccesoDashboard())
            {
                return RedirectConError("No tienes acceso al dashboard", "Home", "Index");
            }

            var usuario = await ObtenerUsuarioActual();
            ViewBag.Usuario = usuario;

            // Preparar datos para el dashboard
            var dashboardData = new DashboardViewModel
            {
                TotalClientes = await _context.Clientes.CountAsync(c => c.EstaActivo),
                TotalEmpleados = await _context.Empleados.CountAsync(e => e.EstaActivo),
                ClientesInactivos = await _context.Clientes.CountAsync(c => !c.EstaActivo),
                EmpleadosInactivos = await _context.Empleados.CountAsync(e => !e.EstaActivo),

                // Clientes recientes (últimos 7 días)
                ClientesRecientes = await _context.Clientes
                    .Where(c => c.FechaInscripcion >= DateTime.Now.AddDays(-7))
                    .CountAsync(),

                // Empleados por cargo
                EmpleadosPorCargo = await _context.Empleados
                    .Where(e => e.EstaActivo)
                    .GroupBy(e => e.Cargo)
                    .Select(g => new CargoCantidad { Cargo = g.Key, Cantidad = g.Count() })
                    .ToListAsync()
            };

            // Verificar permisos específicos
            ViewBag.PuedeGestionarClientes = await PuedeGestionarClientes();
            ViewBag.PuedeGestionarEmpleados = await PuedeGestionarEmpleados();

            return View(dashboardData);
        }

        public async Task<IActionResult> Estadisticas()
        {
            if (!await TieneAccesoDashboard())
            {
                return RedirectConError("No tienes acceso a las estadísticas", "Home", "Index");
            }

            var estadisticas = new EstadisticasViewModel
            {
                // Estadísticas de clientes por mes (últimos 6 meses)
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

                // Empleados por antigüedad
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

    // ViewModels para el Dashboard
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalEmpleados { get; set; }
        public int ClientesInactivos { get; set; }
        public int EmpleadosInactivos { get; set; }
        public int ClientesRecientes { get; set; }
        public List<CargoCantidad> EmpleadosPorCargo { get; set; } = new();
    }

    public class CargoCantidad
    {
        public string Cargo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class EstadisticasViewModel
    {
        public List<MesConteo> ClientesPorMes { get; set; } = new();
        public List<EmpleadoAntiguedad> EmpleadosPorAntiguedad { get; set; } = new();
    }

    public class MesConteo
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Cantidad { get; set; }
        public string NombreMes => new DateTime(Año, Mes, 1).ToString("MMM yyyy");
    }

    public class EmpleadoAntiguedad
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public int AntiguedadDias { get; set; }
        public double AntiguedadAños => Math.Round(AntiguedadDias / 365.0, 1);
    }
}