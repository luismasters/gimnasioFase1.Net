using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppGimn.Models;
using AppGimn.Data;

namespace AppGimn.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ============ REGISTRO GET ============
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ============ REGISTRO POST ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroExtendidoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ============ VALIDACIONES PERSONALIZADAS ============

                // 1. Verificar que no existe usuario con ese email
                var usuarioExistente = await _userManager.FindByEmailAsync(model.Email);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(model);
                }

                // 2. Verificar que no existe usuario con ese DNI
                var usuarioConDNI = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.DNI == model.DNI);
                if (usuarioConDNI != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un usuario con ese DNI");
                    return View(model);
                }

                // 3. Si dice ser empleado, verificar que existe el empleado
                if (model.EsEmpleado)
                {
                    var empleadoExiste = await _context.Empleados
                        .AnyAsync(e => e.DNI == model.DNI && e.EstaActivo);

                    if (!empleadoExiste)
                    {
                        ModelState.AddModelError("DNI",
                            "No se encontró un empleado activo con ese DNI. Contacte al administrador.");
                        return View(model);
                    }
                }

                // 4. Si dice ser cliente, verificar que existe el cliente
                if (model.EsCliente)
                {
                    var clienteExiste = await _context.Clientes
                        .AnyAsync(c => c.DNI == model.DNI && c.EstaActivo);

                    if (!clienteExiste)
                    {
                        ModelState.AddModelError("DNI",
                            "No se encontró un cliente activo con ese DNI. Contacte al administrador.");
                        return View(model);
                    }
                }

                // 5. Debe ser al menos cliente O empleado
                if (!model.EsEmpleado && !model.EsCliente)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un rol: Cliente o Empleado");
                    return View(model);
                }

                // ============ CREAR USUARIO ============
                var usuario = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DNI = model.DNI,
                    EsEmpleado = model.EsEmpleado,
                    EsCliente = model.EsCliente,
                    EsAdmin = false // Los admins solo los crea otro admin
                };

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    // Auto-login después del registro
                    await _signInManager.SignInAsync(usuario, isPersistent: false);

                    TempData["MensajeExito"] = "Cuenta creada exitosamente. ¡Bienvenido al sistema!";
                    return RedirectToAction("Index", "Home");
                }

                // Si hubo errores en la creación, mostrarlos
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }













        // ============ AGREGAR ESTAS ACCIONES A TU AccountController ============

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Intentar hacer login
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Login exitoso
                    TempData["SuccessMessage"] = "¡Bienvenido/a de nuevo!";

                    // Redirigir donde corresponde
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Login falló
                    TempData["ErrorMessage"] = "Email o contraseña incorrectos.";
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Has cerrado sesión correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}