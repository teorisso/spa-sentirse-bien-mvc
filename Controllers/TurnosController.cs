using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Models;
using SpaAdmin.Services;

namespace SpaAdmin.Controllers
{
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
        public async Task<IActionResult> Index(string estado = "todos", string fecha = "")
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
            if (estado != "todos")
                data = data.Where(t => t.Estado == estado).ToList();

            if (!string.IsNullOrWhiteSpace(fecha) &&
                DateOnly.TryParse(fecha, out var f))
                data = data
                        .Where(t => DateOnly.FromDateTime(t.Fecha) == f)
                        .ToList();

            ViewBag.EstadoActual = estado;
            ViewBag.FechaActual  = fecha;

            return View(data);
        }

        // GET: /Turnos/QR/{id}
        [HttpPost]
        public async Task<IActionResult> GenerateQRJson(string id)
        {
            var url = $"{_cfg["ApiSettings:QREndpoint"]}/turno/{id}/checkin";
            var qr = await _api.PostAsync<object, QRCodeResponse>(url, new { });

            if (qr == null) return StatusCode(500, "Error al generar QR");

            return Json(qr);
        }
    }
}