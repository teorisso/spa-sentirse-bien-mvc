using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Data;
using SpaAdmin.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SpaAdmin.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ApplicationDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var filtro = Builders<Cliente>.Filter.Eq(c => c.Role, "cliente");
                var clientes = await _context.Clientes.Find(filtro).ToListAsync();
                _logger.LogInformation("Se obtuvieron {Count} clientes", clientes.Count);
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes");
                TempData["ErrorMessage"] = "Error al cargar la lista de clientes. Por favor, inténtelo de nuevo.";
                return View(new List<Cliente>());
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de cliente no válido");
            }

            try
            {
                var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {Id} no encontrado", id);
                    return NotFound("Cliente no encontrado");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del cliente con ID {Id}", id);
                TempData["ErrorMessage"] = "Error al cargar los detalles del cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que el email no exista
                    var emailExists = await _context.Clientes.Find(c => c.Email == cliente.Email).AnyAsync();
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Ya existe un cliente con este email");
                        return View(cliente);
                    }

                    cliente.Id = string.Empty; // MongoDB genera el Id
                    cliente.Role = "cliente"; // Asignar rol por defecto
                    await _context.Clientes.InsertOneAsync(cliente);
                    
                    _logger.LogInformation("Cliente creado exitosamente: {Email}", cliente.Email);
                    TempData["SuccessMessage"] = "Cliente creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente: {Email}", cliente.Email);
                TempData["ErrorMessage"] = "Error al crear el cliente. Por favor, inténtelo de nuevo.";
                return View(cliente);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de cliente no válido");
            }

            try
            {
                var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {Id} no encontrado para editar", id);
                    return NotFound("Cliente no encontrado");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cliente para editar con ID {Id}", id);
                TempData["ErrorMessage"] = "Error al cargar el cliente para editar.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Cliente cliente)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de cliente no válido");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que el email no exista en otro cliente
                    var emailExists = await _context.Clientes.Find(c => c.Email == cliente.Email && c.Id != id).AnyAsync();
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Ya existe otro cliente con este email");
                        return View(cliente);
                    }

                    var update = Builders<Cliente>.Update
                        .Set(c => c.FirstName, cliente.FirstName)
                        .Set(c => c.LastName, cliente.LastName)
                        .Set(c => c.Email, cliente.Email)
                        .Set(c => c.Role, cliente.Role);
                    
                    var result = await _context.Clientes.UpdateOneAsync(c => c.Id == id, update);
                    
                    if (result.ModifiedCount > 0)
                    {
                        _logger.LogInformation("Cliente actualizado exitosamente: {Id}", id);
                        TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
                    }
                    else
                    {
                        _logger.LogWarning("No se pudo actualizar el cliente: {Id}", id);
                        TempData["WarningMessage"] = "No se realizaron cambios en el cliente.";
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente con ID {Id}", id);
                TempData["ErrorMessage"] = "Error al actualizar el cliente. Por favor, inténtelo de nuevo.";
                return View(cliente);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de cliente no válido");
            }

            try
            {
                var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {Id} no encontrado para eliminar", id);
                    return NotFound("Cliente no encontrado");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar cliente para eliminar con ID {Id}", id);
                TempData["ErrorMessage"] = "Error al cargar el cliente para eliminar.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de cliente no válido");
            }

            try
            {
                var result = await _context.Clientes.DeleteOneAsync(c => c.Id == id);
                
                if (result.DeletedCount > 0)
                {
                    _logger.LogInformation("Cliente eliminado exitosamente: {Id}", id);
                    TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar el cliente: {Id}", id);
                    TempData["WarningMessage"] = "No se pudo eliminar el cliente.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente con ID {Id}", id);
                TempData["ErrorMessage"] = "Error al eliminar el cliente. Por favor, inténtelo de nuevo.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}