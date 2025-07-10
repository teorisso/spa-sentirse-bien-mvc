using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SpaAdmin.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleException(Exception ex, string operation, object? model = null)
        {
            _logger.LogError(ex, "Error en {Operation}: {Message}", operation, ex.Message);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Para peticiones AJAX
                return Json(new { success = false, message = "Error interno del servidor" });
            }
            
            TempData["ErrorMessage"] = $"Error al {operation.ToLower()}. Por favor, inténtelo de nuevo.";
            return RedirectToAction("Index");
        }

        protected void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        protected void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        protected void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        protected IActionResult SuccessRedirect(string action, string message)
        {
            TempData["SuccessMessage"] = message;
            return RedirectToAction(action);
        }

        protected IActionResult ErrorRedirect(string action, string message)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToAction(action);
        }

        protected IActionResult WarningRedirect(string action, string message)
        {
            TempData["WarningMessage"] = message;
            return RedirectToAction(action);
        }

        protected IActionResult InfoRedirect(string action, string message)
        {
            TempData["InfoMessage"] = message;
            return RedirectToAction(action);
        }
    }
} 