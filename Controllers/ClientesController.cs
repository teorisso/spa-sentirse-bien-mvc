using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Data;
using SpaAdmin.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SpaAdmin.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var filtro = Builders<Cliente>.Filter.Eq(c => c.Role, "cliente");
            var clientes = await _context.Clientes.Find(filtro).ToListAsync();
            return View(clientes);
        }

        public async Task<IActionResult> Details(string id)
        {
            var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Id = string.Empty; // MongoDB genera el Id
                await _context.Clientes.InsertOneAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var update = Builders<Cliente>.Update
                    .Set(c => c.FirstName, cliente.FirstName)
                    .Set(c => c.LastName, cliente.LastName)
                    .Set(c => c.Email, cliente.Email)
                    .Set(c => c.Role, cliente.Role);
                await _context.Clientes.UpdateOneAsync(c => c.Id == id, update);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var cliente = await _context.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _context.Clientes.DeleteOneAsync(c => c.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
} 