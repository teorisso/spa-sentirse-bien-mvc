using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Data;
using SpaAdmin.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SpaAdmin.Controllers
{
    public class ProfesionalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfesionalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var profesionales = await _context.Profesionales.Find(Builders<Profesional>.Filter.Empty).ToListAsync();
            return View(profesionales);
        }
    }
} 