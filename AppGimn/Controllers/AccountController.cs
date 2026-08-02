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
                var usuarioExistente = await _userManager.FindByEmailAsync(model.Email);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese email");
                    return View(model);
                }

                var usuarioConDNI = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.DNI == model.DNI);
                if (usuarioConDNI != null)
                {
                    ModelState.AddModelError("DNI", "Ya existe un usuario con ese DNI");
                    return View(model);
                }

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

                if (!model.EsEmpleado && !model.EsCliente)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un rol: Cliente o Empleado");
                    return View(model);
                }

                var usuario = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DNI = model.DNI,
                    EsEmpleado = model.EsEmpleado,
                    EsCliente = model.EsCliente,
                    EsAdmin = false
                };

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    TempData["MensajeExito"] = "Cuenta creada exitosamente. ¡Bienvenido al sistema!";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // ============ LOGIN GET ============
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // ============ LOGIN POST ============
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "¡Bienvenido/a de nuevo!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && user.EsCliente && !user.EsAdmin && !user.EsEmpleado)
                    {
                        return RedirectToAction("ClientePanel", "Dashboard");
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Email o contraseña incorrectos.";
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        // ============ QUICK LOGIN GARANTIZADO EN 1-CLICK PARA LA DEMO ============
        [HttpGet]
        public async Task<IActionResult> QuickLogin(string role)
        {
            await _signInManager.SignOutAsync();

            string cleanRole = role?.ToLower() ?? "admin";

            string email = cleanRole switch
            {
                "cliente" => "cliente@gimnasio.com",
                "recepcion" or "recepcionista" => "recepcion@gimnasio.com",
                "instructor" or "entrenador" => "instructor@gimnasio.com",
                "admin" or "administrador" => "admin@gimnasio.com",
                _ => "admin@gimnasio.com"
            };

            string password = cleanRole switch
            {
                "cliente" => "Cliente123!",
                "recepcion" or "recepcionista" => "Recep123!",
                "instructor" or "entrenador" => "Coach123!",
                "admin" or "administrador" => "Admin123!",
                _ => "Admin123!"
            };

            string dni = cleanRole switch
            {
                "cliente" => "11223344",
                "recepcion" or "recepcionista" => "44556677",
                "instructor" or "entrenador" => "55667788",
                _ => "00000000"
            };

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Usuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DNI = dni,
                    EsAdmin = cleanRole.Contains("admin"),
                    EsEmpleado = !cleanRole.Equals("cliente"),
                    EsCliente = cleanRole.Equals("cliente")
                };
                await _userManager.CreateAsync(user, password);
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return cleanRole switch
            {
                "cliente" => RedirectToAction("ClientePanel", "Dashboard"),
                "recepcion" or "recepcionista" => RedirectToAction("RecepcionPanel", "Dashboard"),
                "instructor" or "entrenador" => RedirectToAction("InstructorPanel", "Dashboard"),
                _ => RedirectToAction("Index", "Dashboard")
            };
        }

        // ============ LOGOUT POST ============
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