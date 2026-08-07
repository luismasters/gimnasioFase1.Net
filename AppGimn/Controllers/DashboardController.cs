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
            ViewBag.ClienteFoto = cliente != null ? cliente.FotoUrl : null;
            ViewBag.MiembroDesde = cliente != null ? cliente.FechaInscripcion.ToString("MMMM yyyy") : "Marzo 2024";

            var ultimoPago = cliente != null 
                ? await _context.Pagos.Include(p => p.Membresia).Where(p => p.ClienteId == cliente.Id).OrderByDescending(p => p.FechaPago).FirstOrDefaultAsync()
                : null;

            ViewBag.MembresiaNombre = ultimoPago?.Membresia != null ? ultimoPago.Membresia.Nombre : "Pase Premium Aura 24/7";
            ViewBag.ProximaClase = "Pilates Reformer - Hoy 18:00 hs";
            ViewBag.EntrenadorAsignado = "Marcus Vance (Head Coach)";

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

        // ============ RUTA 3 - RECEPCIONISTA: TERMINAL DE CHECK-IN (SOLO ENTRADAS) ============
        public async Task<IActionResult> RecepcionPanel()
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var clientesPresentes = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();

            var recaudacionHoy = await _context.Pagos
                .Where(p => p.FechaPago.Date == DateTime.Today)
                .SumAsync(p => (decimal?)p.Monto) ?? 145000m;

            ViewBag.ClientesPresentesCount = clientesPresentes.Count;
            ViewBag.CajaDiaTotal = recaudacionHoy.ToString("C");

            return View(clientesPresentes);
        }

        // ============ RUTA 3 - RECEPCIONISTA: COBROS & GESTIÓN DE VENCIDOS ============
        public async Task<IActionResult> RecepcionCobros(string? busquedaDni)
        {
            var usuario = await ObtenerUsuarioActual();
            if (usuario != null && usuario.EsCliente && !usuario.EsAdmin && !usuario.EsEmpleado)
            {
                return RedirectToAction("ClientePanel");
            }

            var clientes = await _context.Clientes.Where(c => c.EstaActivo).ToListAsync();

            var clienteIds = clientes.Select(c => c.Id).ToList();
            var ultimosPagosList = await _context.Pagos
                .Where(p => clienteIds.Contains(p.ClienteId))
                .GroupBy(p => p.ClienteId)
                .Select(g => g.OrderByDescending(p => p.FechaPago).FirstOrDefault())
                .ToListAsync();

            var ultimosPagosDict = ultimosPagosList
                .Where(p => p != null)
                .ToDictionary(p => p!.ClienteId, p => p!);

            var sociosVencidos = clientes.Where(c => 
                !ultimosPagosDict.ContainsKey(c.Id) || 
                ultimosPagosDict[c.Id].FechaVencimiento < DateTime.Now.Date
            ).ToList();

            Cliente? clienteBuscado = null;
            bool socioExiste = true;

            if (!string.IsNullOrWhiteSpace(busquedaDni))
            {
                string terminoClean = busquedaDni.Trim().ToLower();
                clienteBuscado = await _context.Clientes.FirstOrDefaultAsync(c => 
                    c.DNI == terminoClean || 
                    c.Nombre.ToLower().Contains(terminoClean) || 
                    c.Apellido.ToLower().Contains(terminoClean) ||
                    (c.Nombre + " " + c.Apellido).ToLower().Contains(terminoClean)
                );

                if (clienteBuscado == null)
                {
                    socioExiste = false;
                }
            }

            ViewBag.BusquedaDni = busquedaDni;
            ViewBag.ClienteBuscado = clienteBuscado;
            ViewBag.SocioExiste = socioExiste;
            ViewBag.SociosVencidos = sociosVencidos;
            ViewBag.MembresiasDisponibles = await _context.Membresias.Where(m => m.EstaActivo).ToListAsync();

            var pagosDelDia = await _context.Pagos
                .Include(p => p.Cliente)
                .Include(p => p.Membresia)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            return View(pagosDelDia);
        }

        // ============ REGISTRAR PAGO REAL Y REMOVER DE LA LISTA DE VENCIDOS ============
        [HttpPost]
        public async Task<IActionResult> ProcesarPagoCobro(string dniCliente, int membresiaId, string medioPago)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.DNI == dniCliente);
            if (cliente == null)
            {
                TempData["MensajeError"] = $"No se encontró ningún cliente registrado con el DNI '{dniCliente}'. Debe registrarlo primero.";
                return RedirectToAction("RecepcionCobros", new { busquedaDni = dniCliente });
            }

            var membresia = await _context.Membresias.FindAsync(membresiaId);
            if (membresia == null)
            {
                TempData["MensajeError"] = "Debe seleccionar un plan de membresía válido.";
                return RedirectToAction("RecepcionCobros");
            }

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

            TempData["MensajeExito"] = $"¡Pago de {membresia.Precio:C} procesado con éxito para {cliente.NombreCompleto} (DNI: {cliente.DNI})! Membresía activada hasta el {nuevoPago.FechaVencimiento:dd/MM/yyyy}. El socio ha sido actualizado y removido de la lista de vencidos.";

            return RedirectToAction("RecepcionCobros");
        }

        // ============ VALIDAR CHECK-IN REAL EN MOLINETE Y RETORNAR RESULTADO JSON ============
        [HttpPost]
        public async Task<IActionResult> ValidarCheckinDni(string dni)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.DNI == dni);
            if (cliente == null)
            {
                return Json(new { 
                    ok = false, 
                    estado = "NO_REGISTRADO", 
                    mensaje = "El DNI ingresado no pertenece a ningún socio. ¡Debe registrarse primero!" 
                });
            }

            var ultimoPago = await _context.Pagos
                .Where(p => p.ClienteId == cliente.Id)
                .OrderByDescending(p => p.FechaPago)
                .Include(p => p.Membresia)
                .FirstOrDefaultAsync();

            bool alDia = ultimoPago != null && ultimoPago.FechaVencimiento >= DateTime.Now.Date;

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
                foto = cliente.FotoUrl,
                membresia = ultimoPago?.Membresia?.Nombre ?? "Sin membresía",
                estado = alDia ? "AL_DIA" : "VENCIDO",
                vencimiento = ultimoPago != null ? ultimoPago.FechaVencimiento.ToString("dd/MM/yyyy") : "Sin registro de cuota"
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