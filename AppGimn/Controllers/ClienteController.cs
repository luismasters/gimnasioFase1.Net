using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    [Authorize] // Requiere estar logueado
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ClienteController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ MÉTODO PARA VERIFICAR PERMISOS
        private async Task<bool> PuedeGestionarClientes()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return false;

            // Si es Admin, puede todo
            if (usuario.EsAdmin) return true;

            // Si no es empleado del sistema, no puede gestionar clientes
            if (!usuario.EsEmpleado) return false;

            // Buscar datos del empleado por DNI
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.DNI == usuario.DNI);

            if (empleado == null) return false;

            // Gerentes y Recepcionistas pueden gestionar clientes
            return empleado.Cargo.Equals("Gerente", StringComparison.OrdinalIgnoreCase) ||
                   empleado.Cargo.Equals("Recepcionista", StringComparison.OrdinalIgnoreCase);
        }

        // ✅ INDEX - Lista de clientes con búsqueda por término
        public async Task<IActionResult> Index(string? buscar)
        {
            // Verificar permisos
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para gestionar clientes.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["FiltroActual"] = buscar;

            IQueryable<Cliente> query = _context.Clientes;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var termino = buscar.Trim().ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(termino) ||
                    c.Apellido.ToLower().Contains(termino) ||
                    c.DNI.Contains(termino) ||
                    (c.Email != null && c.Email.ToLower().Contains(termino)));
            }

            var clientes = await query
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .ToListAsync();

            return View(clientes);
        }

        // ✅ DETAILS - Ver detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para ver clientes.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // ✅ CREATE GET - Mostrar formulario
        public async Task<IActionResult> Create()
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para crear clientes.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // ✅ CREATE POST - Procesar formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DNI,Nombre,Apellido,Email,Telefono,FechaNacimiento,FechaInscripcion,ContactoEmergencia,ObservacionesMedicas,EstaActivo")] Cliente cliente)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para crear clientes.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Verificar DNI único
                var existeCliente = await _context.Clientes
                    .AnyAsync(c => c.DNI == cliente.DNI);

                if (existeCliente)
                {
                    ModelState.AddModelError("DNI", "Ya existe un cliente con este DNI.");
                    return View(cliente);
                }

                _context.Add(cliente);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Cliente creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // ✅ EDIT GET - Mostrar formulario de edición
        public async Task<IActionResult> Edit(int? id)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para editar clientes.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // ✅ EDIT POST - Procesar edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DNI,Nombre,Apellido,Email,Telefono,FechaNacimiento,FechaInscripcion,ContactoEmergencia,ObservacionesMedicas,EstaActivo")] Cliente cliente)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para editar clientes.";
                return RedirectToAction("Index", "Home");
            }

            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar DNI único (excepto el actual)
                    var existeCliente = await _context.Clientes
                        .AnyAsync(c => c.DNI == cliente.DNI && c.Id != cliente.Id);

                    if (existeCliente)
                    {
                        ModelState.AddModelError("DNI", "Ya existe un cliente con este DNI.");
                        return View(cliente);
                    }

                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = "Cliente actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // ✅ DELETE GET - Confirmar eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para eliminar clientes.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // ✅ DELETE POST - Procesar eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await PuedeGestionarClientes())
            {
                TempData["Error"] = "No tienes permisos para eliminar clientes.";
                return RedirectToAction("Index", "Home");
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Cliente eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ MÉTODO AUXILIAR
        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(c => c.Id == id);
        }
    }
}