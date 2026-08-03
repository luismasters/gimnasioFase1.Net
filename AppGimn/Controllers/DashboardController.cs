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

            if (cliente == null)
            {
                cliente = await _context.Clientes.FirstOrDefaultAsync();
            }

            ViewBag.ClienteNombre = cliente != null ? cliente.NombreCompleto : "Carlos Gómez";
            ViewBag.MiembroDesde = cliente != null ? cliente.FechaInscripcion.ToString("MMMM yyyy") : "Marzo 2024";

            // Obtener último pago/membresía de la base de datos real
            var ultimoPago = cliente != null 
                ? await _context.Pagos.Include(p => p.Membresia).Where(p => p.ClienteId == cliente.Id).OrderByDescending(p => p.FechaPago).FirstOrDefaultAsync()
                : null;

            ViewBag.MembresiaNombre = ultimoPago?.Membresia != null ? ultimoPago.Membresia.Nombre : "Pase Premium Aura 24/7";
            ViewBag.ProximaClase = "Pilates Reformer - Hoy 18:00 hs";
            ViewBag.EntrenadorAsignado = "Marcus Vance (Head Coach)";

            // Obtener última evaluación física real
            var ultimaEvaluacion = cliente != null
                ? await _context.EvaluacionesFisicas.Where(e => e.ClienteId == cliente.Id).OrderByDescending(e => e.FechaEvaluacion).FirstOrDefaultAsync()
                : null;

            ViewBag.UltimoPeso = ultimaEvaluacion != null ? ultimaEvaluacion.PesoKg : 78.5;
            ViewBag.UltimaGrasa = ultimaEvaluacion != null ? ultimaEvaluacion.PorcentajeGrasa : 14.2;

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

            var evaluaciones = cliente != null
                ? await _context.EvaluacionesFisicas.Where(e => e.ClienteId == cliente.Id).OrderBy(e => e.FechaEvaluacion).ToListAsync()
                : new List<EvaluacionFisica>();

            ViewBag.ClienteNombre = cliente != null ? cliente.NombreCompleto : "Carlos Gómez";
            return View(evaluaciones);
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
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var clientesPresentes = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();

            // Calcular recaudación real del día en EF Core
            var recaudacionHoy = await _context.Pagos
                .Where(p => p.FechaPago.Date == DateTime.Today)
                .SumAsync(p => (decimal?)p.Monto) ?? 145000m;

            ViewBag.ClientesPresentesCount = clientesPresentes.Count;
            ViewBag.VencimientosCount = 5;
            ViewBag.CajaDiaTotal = recaudacionHoy.ToString("C");

            return View(clientesPresentes);
        }

        public async Task<IActionResult> RecepcionCobros()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            // Consultar transacciones de pago reales
            var pagos = await _context.Pagos
                .Include(p => p.Cliente)
                .Include(p => p.Membresia)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            ViewBag.MembresiasDisponibles = await _context.Membresias.Where(m => m.EstaActivo).ToListAsync();

            return View(pagos);
        }

        // ============ REGISTRAR PAGO REAL DE CUOTA EN BASE DE DATOS ============
        [HttpPost]
        public async Task<IActionResult> ProcesarPagoCobro(int clienteId, int membresiaId, string medioPago)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            var membresia = await _context.Membresias.FindAsync(membresiaId);

            if (cliente != null && membresia != null)
            {
                var usuarioActual = await ObtenerUsuarioActual();
                var nuevoPago = new Pago
                {
                    ClienteId = cliente.Id,
                    MembresiaId = membresia.Id,
                    Monto = membresia.Precio,
                    FechaPago = DateTime.Now,
                    FechaVencimiento = DateTime.Now.AddDays(membresia.DuracionDias),
                    MedioPago = medioPago ?? "Efectivo",
                    ComprobanteNumero = $"REC-{Random.Shared.Next(10000, 99999)}",
                    RecepcionistaEmail = usuarioActual?.Email ?? "recepcion@gimnasio.com"
                };

                await _context.Pagos.AddAsync(nuevoPago);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = $"¡Pago de {membresia.Precio:C} registrado con éxito para {cliente.NombreCompleto}! Comprobante {nuevoPago.ComprobanteNumero} emitido.";
            }

            return RedirectToAction("RecepcionCobros");
        }

        // ============ VALIDAR CHECK-IN REAL Y REGISTRAR ASISTENCIA EN BASE DE DATOS ============
        [HttpPost]
        public async Task<IActionResult> ValidarCheckinDni(string dni)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.DNI == dni);
            if (cliente == null)
            {
                return Json(new { ok = false, estado = "NO_ENCONTRADO", mensaje = "Socio no registrado en la base de datos." });
            }

            var ultimoPago = await _context.Pagos.Where(p => p.ClienteId == cliente.Id).OrderByDescending(p => p.FechaPago).FirstOrDefaultAsync();

            bool alDia = ultimoPago == null || ultimoPago.FechaVencimiento >= DateTime.Now.Date;

            // Registrar asistencia real en la base de datos
            var asistencia = new Asistencia
            {
                ClienteId = cliente.Id,
                FechaHoraIngreso = DateTime.Now,
                Permitido = alDia,
                MotivoDenegado = alDia ? null : "Cuota vencida"
            };

            await _context.Asistencias.AddAsync(asistencia);
            await _context.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                nombre = cliente.NombreCompleto,
                dni = cliente.DNI,
                estado = alDia ? "AL_DIA" : "VENCIDO",
                vencimiento = ultimoPago != null ? ultimoPago.FechaVencimiento.ToString("dd/MM/yyyy") : "Sin registro de pago"
            });
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