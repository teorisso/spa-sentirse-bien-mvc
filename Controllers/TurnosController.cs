using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Models;
using SpaAdmin.Services;

namespace SpaAdmin.Controllers
{
    [Authorize]
    public class TurnosController : Controller
    {
        private readonly IApiClient _api;
        private readonly IConfiguration _cfg;
        private readonly ILogger<TurnosController> _log;

        public TurnosController(IApiClient api, IConfiguration cfg, ILogger<TurnosController> log)
        {
            _api = api; _cfg = cfg; _log = log;
        }

        // GET: /Turnos
        public async Task<IActionResult> Index(string estado = "todos", string fecha = "", string cliente = "")
        {
            var url = $"{_cfg["ApiSettings:TurnosEndpoint"]}?page=1&pageSize=100";
            var result = await _api.GetDirectAsync<TurnosPaginated>(url);

            if (result == null || !result.Success)
            {
                TempData["Error"] = "No se pudieron obtener los turnos";
                return View(new List<TurnoDto>());
            }

            // filtros simples en memoria (lado cliente)
            var data = result.Data;
            
            // Filtro por estado
            if (estado != "todos")
                data = data.Where(t => t.Estado == estado).ToList();

            // Filtro por fecha
            if (!string.IsNullOrWhiteSpace(fecha) &&
                DateOnly.TryParse(fecha, out var f))
                data = data
                        .Where(t => DateOnly.FromDateTime(t.Fecha) == f)
                        .ToList();

            // Filtro por cliente (búsqueda por nombre)
            if (!string.IsNullOrWhiteSpace(cliente))
            {
                data = data.Where(t => 
                    (t.Cliente?.FirstName?.Contains(cliente, StringComparison.OrdinalIgnoreCase) == true) ||
                    (t.Cliente?.LastName?.Contains(cliente, StringComparison.OrdinalIgnoreCase) == true) ||
                    ($"{t.Cliente?.FirstName} {t.Cliente?.LastName}".Contains(cliente, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.EstadoActual = estado;
            ViewBag.FechaActual  = fecha;
            ViewBag.ClienteActual = cliente;

            return View(data);
        }

        // post: /Turnos/GetQRJson
        [HttpPost]
        public async Task<IActionResult> GetQRJson(string id)
        {
            try
            {
                _log.LogInformation("Solicitando QR para turno: {TurnoId}", id);
                
                var url = $"{_cfg["ApiSettings:QREndpoint"]}/turno/{id}/checkin";
                _log.LogInformation("URL del QR: {Url}", url);
                
                var qr = await _api.PostAsync<object, QRCodeResponse>(url, new { });

                if (qr == null)
                {
                    _log.LogError("La API devolvió null para el turno: {TurnoId}", id);
                    return Json(new { 
                        success = false, 
                        message = "No se pudo obtener el QR. Verifique que el turno esté en la ventana de tiempo correcta." 
                    });
                }

                _log.LogInformation("QR obtenido exitosamente para turno: {TurnoId}", id);
                
                // Devolver estructura que espera el JavaScript
                return Json(new { 
                    success = true, 
                    data = qr,
                    message = "QR obtenido exitosamente" 
                });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error al obtener QR para turno: {TurnoId}", id);
                return Json(new { 
                    success = false, 
                    message = $"Error interno: {ex.Message}" 
                });
            }
        }

        // POST: /Turnos/UpdateEstado
        [HttpPost]
        public async Task<IActionResult> UpdateEstado(string id, string estado)
        {
            try
            {
                _log.LogInformation("Actualizando estado del turno: {TurnoId} a {Estado}", id, estado);
                
                var url = $"{_cfg["ApiSettings:TurnosEndpoint"]}/{id}";
                var updateData = new { Estado = estado };
                
                _log.LogInformation("Enviando PUT a: {Url} with data: {@Data}", url, updateData);
                
                var result = await _api.PutAsync<object, TurnoDto>(url, updateData);

                if (result != null)
                {
                    _log.LogInformation("Estado actualizado exitosamente para turno: {TurnoId}", id);
                    return Json(new { 
                        success = true, 
                        message = "Estado actualizado exitosamente" 
                    });
                }
                else
                {
                    _log.LogError("Error al actualizar estado del turno: {TurnoId} - API devolvió null", id);
                    return Json(new { 
                        success = false, 
                        message = "No se pudo actualizar el estado del turno. Verifique que el turno no esté en estado 'realizado' o que tenga permisos de administrador." 
                    });
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error al actualizar estado del turno: {TurnoId}", id);
                return Json(new { 
                    success = false, 
                    message = $"Error interno: {ex.Message}" 
                });
            }
        }
    }
}