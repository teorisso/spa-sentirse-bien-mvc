using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Services;

namespace SpaAdmin.Controllers
{
    public class ApiTestController : Controller
    {
        private readonly IApiAuthService _authService;

        public ApiTestController(IApiAuthService authService)
        {
            _authService = authService;
        }

        // GET /ApiTest/Login
        public async Task<IActionResult> Login()
        {
            var ok = await _authService.LoginAsync();

            if (ok && _authService.Token != null)
            {
                return Content($"✅ Login OK – token guardado, vence en {_authService.TokenExpiry}");
            }

            return Content("❌ Login FALLÓ – revisá credenciales o que la API esté corriendo.");
        }
    }
}