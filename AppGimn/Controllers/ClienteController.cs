using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppGimn.Data;
using AppGimn.Models;

namespace AppGimn.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ LISTAR CLIENTES ============
        public async Task<IActionResult> Index(string buscar)
        {
            ViewData["FiltroActual"] = buscar;

            IQueryable<Cliente> clientes;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                // Usar el método que creaste en DbContext
                clientes = _context.BuscarClientes(buscar);
            }
            else
            {
                // Solo clientes activos por defecto
                clientes = _context.ClientesActivos;
            }

            return View(await clientes.ToListAsync());
        }

        // ============ VER DETALLES CLIENTE ============
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // ============ CREAR CLIENTE - GET ============
        public IActionResult Create()
        {
            var cliente = new Cliente
            {
                FechaInscripcion = DateTime.Now,
                EstaActivo = true
            };

            return View(cliente);
        }

        // ============ CREAR CLIENTE - POST ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Verificar que no existe otro cliente con el mismo DNI
                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.DNI == cliente.DNI);

                if (clienteExistente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un cliente con ese DNI");
                    return View(cliente);
                }

                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = $"Cliente {cliente.NombreCompleto} creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            return View(cliente);
        }

        // ============ EDITAR CLIENTE - GET ============
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // ============ EDITAR CLIENTE - POST ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar que no existe otro cliente con el mismo DNI (excepto él mismo)
                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.DNI == cliente.DNI && c.Id != cliente.Id);

                if (clienteExistente != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe otro cliente con ese DNI");
                    return View(cliente);
                }

                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    TempData["MensajeExito"] = $"Cliente {cliente.NombreCompleto} actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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

            return View(cliente);
        }

        // ============ ELIMINAR CLIENTE - GET ============
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // ============ ELIMINAR CLIENTE - POST ============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                // Borrado lógico (marcar como inactivo)
                cliente.EstaActivo = false;
                _context.Update(cliente);

                // Si querés borrado físico, usá esto en su lugar:
                // _context.Clientes.Remove(cliente);

                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = $"Cliente {cliente.NombreCompleto} eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ REACTIVAR CLIENTE ============
        [HttpPost]
        public async Task<IActionResult> Reactivar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                cliente.EstaActivo = true;
                _context.Update(cliente);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = $"Cliente {cliente.NombreCompleto} reactivado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============ MÉTODOS AUXILIARES ============
        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}