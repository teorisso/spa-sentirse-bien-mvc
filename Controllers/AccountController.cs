using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SpaAdmin.Models;
using SpaAdmin.Services;
using System.Security.Claims;

namespace SpaAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiAuthService _apiAuthService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IApiAuthService apiAuthService, ILogger<AccountController> logger)
        {
            _apiAuthService = apiAuthService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Si ya está autenticado, redirigir al home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _apiAuthService.LoginUserAsync(model);

                if (result?.Success == true && result.Data?.User != null)
                {
                    var user = result.Data.User;

                    // Verificar que el usuario sea admin o profesional
                    if (user.Role != "admin" && user.Role != "profesional")
                    {
                        ModelState.AddModelError(string.Empty, "Acceso denegado. Solo administradores y profesionales pueden acceder.");
                        return View(model);
                    }

                    // Crear claims para la cookie de autenticación
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.Id),
                        new(ClaimTypes.Name, user.FullName),
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.Role, user.Role),
                        new("FirstName", user.FirstName),
                        new("LastName", user.LastName),
                        new("IsAdmin", user.IsAdmin.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Recordar sesión
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("Usuario autenticado exitosamente: {Email} - {Role}", user.Email, user.Role);

                    TempData["SuccessMessage"] = $"¡Bienvenido, {user.FirstName}!";

                    // Redirigir a la URL original o al home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Credenciales inválidas");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor. Por favor, inténtalo nuevamente.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Si ya está autenticado, redirigir al home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verificar que el rol sea válido para el panel administrativo
            if (model.Role != "admin" && model.Role != "profesional")
            {
                ModelState.AddModelError("Role", "Rol inválido. Solo se permiten administradores y profesionales.");
                return View(model);
            }

            try
            {
                var result = await _apiAuthService.RegisterUserAsync(model);

                if (result?.Success == true && result.Data?.User != null)
                {
                    _logger.LogInformation("Usuario registrado exitosamente: {Email} - {Role}", model.Email, model.Role);

                    TempData["SuccessMessage"] = "Usuario registrado exitosamente. Ya puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Error al registrar el usuario");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor. Por favor, inténtalo nuevamente.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _apiAuthService.ForgotPasswordAsync(model);

                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = "Si el email existe, se ha enviado un enlace de recuperación.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Error al solicitar recuperación de contraseña");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante forgot password: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor. Por favor, inténtalo nuevamente.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Token de recuperación inválido.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordDto { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _apiAuthService.ResetPasswordAsync(model);

                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Error al restablecer la contraseña");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante reset password");
                ModelState.AddModelError(string.Empty, "Error interno del servidor. Por favor, inténtalo nuevamente.");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            _logger.LogInformation("Usuario desautenticado: {Email}", userEmail);
            
            TempData["InfoMessage"] = "Sesión cerrada exitosamente.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 