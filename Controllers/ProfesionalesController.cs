using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Models;
using SpaAdmin.Services;

namespace SpaAdmin.Controllers
{
    [Authorize(Policy = "AdminOrProfessional")]
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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Crear nuevo profesional
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
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

        /// <summary>
        /// Mostrar detalles de un profesional específico
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "ID de profesional inválido";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener profesional de la API
                var profesional = await _apiClient.GetAsync<UserDto>($"users/{id}");

                if (profesional == null)
                {
                    TempData["Error"] = "Profesional no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"Mostrando detalles del profesional: {profesional.FullName}");
                return View(profesional);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del profesional: {Id}", id);
                TempData["Error"] = "Error al cargar los detalles del profesional";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Mostrar vista para editar profesional
        /// </summary>
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "ID de profesional inválido";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener profesional de la API
                var profesional = await _apiClient.GetAsync<UserDto>($"users/{id}");

                if (profesional == null)
                {
                    TempData["Error"] = "Profesional no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Mapear a modelo de edición
                var editModel = new EditProfesionalDto
                {
                    Id = profesional.Id,
                    FirstName = profesional.FirstName,
                    LastName = profesional.LastName,
                    Email = profesional.Email,
                    Role = profesional.Role
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el profesional para editar: {Id}", id);
                TempData["Error"] = "Error al cargar los datos del profesional";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualizar profesional
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(EditProfesionalDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Preparar datos para la API
                var updateDto = new UpdateProfesionalDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Role = model.Role
                };

                // Enviar a la API para actualización
                var result = await _apiClient.PutAsync<UpdateProfesionalDto, UserDto>($"users/{model.Id}", updateDto);

                if (result != null)
                {
                    TempData["Success"] = "Profesional actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Error al actualizar el profesional. Verifique que el email no esté en uso.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar profesional: {Id}", model.Id);
                TempData["Error"] = "Error interno al actualizar el profesional. Intente nuevamente.";
                return View(model);
            }
        }

        /// <summary>
        /// Eliminar profesional
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Solicitud de eliminación recibida para el profesional: {Id}", id);

                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID de profesional inválido: {Id}", id);
                    TempData["Error"] = "ID de profesional inválido";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("Enviando solicitud DELETE a la API para el usuario: {Id}", id);

                // Eliminar desde la API
                var result = await _apiClient.DeleteAsync($"users/{id}");

                _logger.LogInformation("Resultado de eliminación desde API: {Result}", result);

                if (result)
                {
                    _logger.LogInformation("Profesional eliminado exitosamente: {Id}", id);
                    TempData["Success"] = "Profesional eliminado exitosamente";
                }
                else
                {
                    _logger.LogWarning("Error al eliminar el profesional desde la API: {Id}", id);
                    TempData["Error"] = "Error al eliminar el profesional. Puede que tenga turnos activos o permisos insuficientes.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar profesional: {Id}", id);
                TempData["Error"] = "Error interno al eliminar el profesional. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
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

    /// <summary>
    /// DTO para editar profesional
    /// </summary>
    public class EditProfesionalDto
    {
        public string Id { get; set; } = string.Empty;

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

        [System.ComponentModel.DataAnnotations.Display(Name = "Rol")]
        public string Role { get; set; } = "profesional";
    }

    /// <summary>
    /// DTO para actualizar profesional en la API
    /// </summary>
    public class UpdateProfesionalDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
} 