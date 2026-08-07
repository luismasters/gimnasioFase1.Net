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
        private readonly IWebHostEnvironment _env;

        public ClienteController(ApplicationDbContext context, UserManager<Usuario> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // ✅ GUARDAR FOTO SUBIDA DEL CLIENTE EN WWWROOT/UPLOADS/FOTOS
        private async Task<string?> GuardarFoto(IFormFile? foto)
        {
            if (foto == null || foto.Length == 0) return null;

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError("Foto", "El archivo debe ser una imagen (JPG, PNG, WebP, GIF o BMP).");
                return null;
            }

            if (foto.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("Foto", "La imagen no puede superar los 5 MB.");
                return null;
            }

            var carpetaUploads = Path.Combine(_env.WebRootPath, "uploads", "fotos");
            Directory.CreateDirectory(carpetaUploads);

            var nombreArchivo = $"cliente_{Guid.NewGuid():N}{extension}";
            var rutaCompleta = Path.Combine(carpetaUploads, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            return $"/uploads/fotos/{nombreArchivo}";
        }

        // ✅ ELIMINAR FOTO ANTERIOR DEL DISCO
        private void EliminarFoto(string? fotoUrl)
        {
            if (string.IsNullOrWhiteSpace(fotoUrl) || !fotoUrl.StartsWith("/uploads/fotos/")) return;

            var rutaCompleta = Path.Combine(_env.WebRootPath, fotoUrl.TrimStart('/'));
            if (System.IO.File.Exists(rutaCompleta))
            {
                System.IO.File.Delete(rutaCompleta);
            }
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

            // Datos de suscripción (último pago) para mostrar plan y estado de la cuota
            var clienteIds = clientes.Select(c => c.Id).ToList();
            var ultimosPagos = await _context.Pagos
                .Where(p => clienteIds.Contains(p.ClienteId))
                .Include(p => p.Membresia)
                .GroupBy(p => p.ClienteId)
                .Select(g => g.OrderByDescending(p => p.FechaPago).FirstOrDefault())
                .ToListAsync();

            var planDict = new Dictionary<int, string>();
            var estadoDict = new Dictionary<int, bool>();
            foreach (var p in ultimosPagos.Where(p => p != null))
            {
                planDict[p!.ClienteId] = p.Membresia?.Nombre ?? "Sin plan";
                estadoDict[p!.ClienteId] = p.FechaVencimiento >= DateTime.Now.Date;
            }
            ViewBag.PlanDict = planDict;
            ViewBag.EstadoSuscripcionDict = estadoDict;

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
        public async Task<IActionResult> Create([Bind("DNI,Nombre,Apellido,Email,Telefono,FechaNacimiento,FechaInscripcion,ContactoEmergencia,ObservacionesMedicas,EstaActivo,FotoUrl")] Cliente cliente, IFormFile? Foto)
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

                cliente.FotoUrl = await GuardarFoto(Foto);

                if (!ModelState.IsValid)
                {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,DNI,Nombre,Apellido,Email,Telefono,FechaNacimiento,FechaInscripcion,ContactoEmergencia,ObservacionesMedicas,EstaActivo,FotoUrl")] Cliente cliente, IFormFile? Foto)
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

                    var clienteAnterior = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cliente.Id);

                    if (Foto != null && Foto.Length > 0)
                    {
                        EliminarFoto(clienteAnterior?.FotoUrl);
                        cliente.FotoUrl = await GuardarFoto(Foto);
                        if (!ModelState.IsValid)
                        {
                            return View(cliente);
                        }
                    }
                    else
                    {
                        // Conservar la foto existente si no se subió una nueva
                        cliente.FotoUrl = clienteAnterior?.FotoUrl;
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
                EliminarFoto(cliente.FotoUrl);
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