using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    [Authorize] // Requiere estar logueado
    public class EmpleadoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ MÉTODO PARA VERIFICAR PERMISOS
        private async Task<bool> PuedeGestionarEmpleados()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return false;

            // Si es Admin, puede todo
            if (usuario.EsAdmin) return true;

            // Si no es empleado del sistema, no puede
            if (!usuario.EsEmpleado) return false;

            // Buscar datos del empleado por DNI
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI);

            if (empleado == null) return false;

            // Solo Gerentes pueden gestionar empleados
            return empleado.Cargo.Equals("Gerente", StringComparison.OrdinalIgnoreCase);
        }

        // ✅ INDEX - Lista de empleados con filtrado y búsqueda
        public async Task<IActionResult> Index(string? buscar, string? cargo)
        {
            // Verificar permisos
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para gestionar empleados.";
                return RedirectToAction("Index", "Home");
            }

            // Cargar lista de cargos para el filtro desplegable
            ViewData["CargosDisponibles"] = await _context.Empleados
                .Where(e => !string.IsNullOrEmpty(e.Cargo))
                .Select(e => e.Cargo)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewData["FiltroActual"] = buscar;
            ViewData["FiltroCargo"] = cargo;

            IQueryable<Empleado> query = _context.Empleados;

            // Filtro por término de búsqueda (Nombre, Apellido, DNI, Email)
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var termino = buscar.Trim().ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(termino) ||
                    e.Apellido.ToLower().Contains(termino) ||
                    e.DNI.Contains(termino) ||
                    (e.Email != null && e.Email.ToLower().Contains(termino)));
            }

            // Filtro por cargo específico
            if (!string.IsNullOrWhiteSpace(cargo))
            {
                query = query.Where(e => e.Cargo == cargo);
            }

            var empleados = await query
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            return View(empleados);
        }

        // ✅ DETAILS - Ver detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para ver empleados.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // ✅ CREATE GET - Mostrar formulario
        public async Task<IActionResult> Create()
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para crear empleados.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // ✅ CREATE POST - Procesar formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,Nombre,Apellido,Email,Telefono,FechaIngreso,Cargo,Salario,Observaciones")] Empleado empleado)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para crear empleados.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Verificar DNI único
                var existeEmpleado = await _context.Empleados
                    .AnyAsync(e => e.DNI == empleado.DNI);

                if (existeEmpleado)
                {
                    ModelState.AddModelError("DNI", "Ya existe un empleado con este DNI.");
                    return View(empleado);
                }

                _context.Add(empleado);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Empleado creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // ✅ EDIT GET - Mostrar formulario de edición
        public async Task<IActionResult> Edit(int? id)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para editar empleados.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // ✅ EDIT POST - Procesar edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DNI,Nombre,Apellido,Email,Telefono,FechaIngreso,Cargo,Salario,EstaActivo,Observaciones")] Empleado empleado)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para editar empleados.";
                return RedirectToAction("Index", "Home");
            }

            if (id != empleado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar DNI único (excepto el actual)
                    var existeEmpleado = await _context.Empleados
                        .AnyAsync(e => e.DNI == empleado.DNI && e.Id != empleado.Id);

                    if (existeEmpleado)
                    {
                        ModelState.AddModelError("DNI", "Ya existe un empleado con este DNI.");
                        return View(empleado);
                    }

                    _context.Update(empleado);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = "Empleado actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // ✅ DELETE GET - Confirmar eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para eliminar empleados.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // ✅ DELETE POST - Procesar eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await PuedeGestionarEmpleados())
            {
                TempData["Error"] = "No tienes permisos para eliminar empleados.";
                return RedirectToAction("Index", "Home");
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Empleado eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ MÉTODO AUXILIAR
        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}