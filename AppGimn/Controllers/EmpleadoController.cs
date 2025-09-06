using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Lista de cargos disponibles - podría venir de BD en el futuro
        private readonly List<string> _cargosDisponibles = new()
        {
            "Recepcionista",
            "Instructor",
            "Personal Trainer",
            "Gerente",
            "Mantenimiento"
        };

        public EmpleadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ LISTAR EMPLEADOS ============
        public async Task<IActionResult> Index(string buscar, string filtrarCargo)
        {
            ViewData["FiltroActual"] = buscar;
            ViewData["FiltroCargo"] = filtrarCargo;
            ViewData["CargosDisponibles"] = _cargosDisponibles;

            IQueryable<Empleado> empleados = _context.Empleados.Where(e => e.EstaActivo);

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                empleados = empleados.Where(e =>
                    e.Nombre.Contains(buscar) ||
                    e.Apellido.Contains(buscar) ||
                    e.DNI.Contains(buscar) ||
                    e.Email.Contains(buscar));
            }

            // Filtro por cargo
            if (!string.IsNullOrWhiteSpace(filtrarCargo) && filtrarCargo != "Todos")
            {
                empleados = empleados.Where(e => e.Cargo == filtrarCargo);
            }

            // Ordenar por fecha de contratación (más recientes primero)
            empleados = empleados.OrderByDescending(e => e.FechaIngreso);

            return View(await empleados.ToListAsync());
        }

        // ============ VER DETALLES EMPLEADO ============
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // ============ CREAR EMPLEADO - GET ============
        public IActionResult Create()
        {
            ViewData["CargosDisponibles"] = _cargosDisponibles;

            var empleado = new Empleado
            {
                FechaIngreso = DateTime.Now,
                EstaActivo = true,
                Salario = 0 // Se definirá según el cargo
            };

            return View(empleado);
        }

        // ============ CREAR EMPLEADO - POST ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                // Verificar que no existe otro empleado con el mismo DNI
                var empleadoExistente = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.DNI == empleado.DNI);

                if (empleadoExistente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un empleado con ese DNI");
                    ViewData["CargosDisponibles"] = _cargosDisponibles;
                    return View(empleado);
                }

                // Validar que el cargo sea válido
                if (!_cargosDisponibles.Contains(empleado.Cargo))
                {
                    ModelState.AddModelError("Cargo", "Cargo no válido");
                    ViewData["CargosDisponibles"] = _cargosDisponibles;
                    return View(empleado);
                }

                try
                {
                    _context.Add(empleado);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = $"Empleado {empleado.NombreCompleto} ({empleado.Cargo}) creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            ViewData["CargosDisponibles"] = _cargosDisponibles;
            return View(empleado);
        }

        // ============ EDITAR EMPLEADO - GET ============
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            ViewData["CargosDisponibles"] = _cargosDisponibles;
            return View(empleado);
        }

        // ============ EDITAR EMPLEADO - POST ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar que no existe otro empleado con el mismo DNI (excepto él mismo)
                var empleadoExistente = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.DNI == empleado.DNI && e.Id != empleado.Id);

                if (empleadoExistente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe otro empleado con ese DNI");
                    ViewData["CargosDisponibles"] = _cargosDisponibles;
                    return View(empleado);
                }

                // Validar que el cargo sea válido
                if (!_cargosDisponibles.Contains(empleado.Cargo))
                {
                    ModelState.AddModelError("Cargo", "Cargo no válido");
                    ViewData["CargosDisponibles"] = _cargosDisponibles;
                    return View(empleado);
                }

                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = $"Empleado {empleado.NombreCompleto} actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                }
            }

            ViewData["CargosDisponibles"] = _cargosDisponibles;
            return View(empleado);
        }

        // ============ ELIMINAR EMPLEADO - GET ============
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // ============ ELIMINAR EMPLEADO - POST ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado != null)
            {
                // Borrado lógico (marcar como inactivo) - igual que Cliente
                empleado.EstaActivo = false;
                _context.Update(empleado);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = $"Empleado {empleado.NombreCompleto} desactivado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ REACTIVAR EMPLEADO ============
        [HttpPost]
        public async Task<IActionResult> Reactivar(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado != null)
            {
                empleado.EstaActivo = true;
                _context.Update(empleado);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = $"Empleado {empleado.NombreCompleto} reactivado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ REPORTES BÁSICOS ============
        public async Task<IActionResult> Reportes()
        {
            var empleadosActivos = await _context.Empleados
                .Where(e => e.EstaActivo)
                .ToListAsync();

            var reporteBasico = new
            {
                TotalEmpleados = empleadosActivos.Count,
                EmpleadosPorCargo = empleadosActivos
                    .GroupBy(e => e.Cargo)
                    .Select(g => new { Cargo = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .ToList(),
                PromedioSalario = empleadosActivos.Average(e => e.Salario),
                SalarioTotal = empleadosActivos.Sum(e => e.Salario),
                EmpleadoMasAntiguo = empleadosActivos
                    .OrderBy(e => e.FechaIngreso)
                    .FirstOrDefault(),
                EmpleadoMasReciente = empleadosActivos
                    .OrderByDescending(e => e.FechaIngreso)
                    .FirstOrDefault()
            };

            ViewBag.Reporte = reporteBasico;
            return View(empleadosActivos);
        }

        // ============ MÉTODOS AUXILIARES ============
        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}