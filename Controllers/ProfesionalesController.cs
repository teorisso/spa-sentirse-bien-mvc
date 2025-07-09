using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Models;
using SpaAdmin.Services;

namespace SpaAdmin.Controllers
{
    public class ProfesionalesController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<ProfesionalesController> _logger;

        public ProfesionalesController(IApiClient apiClient, ILogger<ProfesionalesController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Mostrar listado de profesionales
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Consumir endpoint específico para profesionales
                var profesionales = await _apiClient.GetAsync<List<UserDto>>("users/profesionales");

                if (profesionales == null)
                {
                    _logger.LogWarning("No se pudieron obtener los profesionales de la API");
                    profesionales = new List<UserDto>();
                    TempData["Error"] = "No se pudieron cargar los profesionales. Verifique la conexión con la API.";
                }

                _logger.LogInformation($"Se obtuvieron {profesionales.Count} profesionales de la API");
                return View(profesionales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener profesionales de la API");
                TempData["Error"] = "Error al cargar los profesionales. Por favor, intente nuevamente.";
                return View(new List<UserDto>());
            }
        }

        /// <summary>
        /// Mostrar vista para crear nuevo profesional
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crear nuevo profesional
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProfesionalDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Preparar datos para la API
                var registerDto = new RegisterProfesionalDto
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Email = model.Email!,
                    Password = model.Password!,
                    Role = "profesional"
                };

                // Enviar a la API para registro
                var result = await _apiClient.PostAsync<RegisterProfesionalDto, AuthResponseDto>("auth/register", registerDto);

                if (result != null)
                {
                    TempData["Success"] = "Profesional creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error al crear el profesional. Verifique que el email no esté en uso.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear profesional");
                TempData["Error"] = "Error interno al crear el profesional. Intente nuevamente.";
                return View(model);
            }
        }
    }

    /// <summary>
    /// DTO para crear profesional
    /// </summary>
    public class CreateProfesionalDto
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El nombre es obligatorio")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Nombre")]
        public string? FirstName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El apellido es obligatorio")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Apellido")]
        public string? LastName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El email es obligatorio")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "El email no es válido")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Email")]
        public string? Email { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contraseña es obligatoria")]
        [System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Contraseña")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// DTO para registro en la API
    /// </summary>
    public class RegisterProfesionalDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "profesional";
    }

    /// <summary>
    /// DTO para respuesta de autenticación
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
} 