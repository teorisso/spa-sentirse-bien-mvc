using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using SpaAdmin.Data;
using SpaAdmin.Models;

public class ServiciosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiciosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, string categoria = "")
    {
        var pageSize = 10;
        var skip = (page - 1) * pageSize;

        // Crear filtro para tipo (que mapea a categoria) si se especifica
        var filter = string.IsNullOrEmpty(categoria) 
            ? Builders<Servicio>.Filter.Empty 
            : Builders<Servicio>.Filter.Eq(s => s.Tipo, categoria);

        // Obtener servicios con paginación
        var servicios = await _context.Servicios
            .Find(filter)
            .SortBy(s => s.Nombre)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        // Obtener total para paginación
        var total = await _context.Servicios.CountDocumentsAsync(filter);

        // Pasar datos de paginación a la vista
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        ViewBag.CurrentCategoria = categoria;

        // Obtener tipos únicos para filtro (equivalente a categorías)
        var categorias = await _context.Servicios
            .Distinct<string>("tipo", Builders<Servicio>.Filter.Empty)
            .ToListAsync();
        
        // Filtrar valores nulos y vacíos
        categorias = categorias.Where(c => !string.IsNullOrEmpty(c)).ToList();
        ViewBag.Categorias = categorias;

        return View(servicios);
    }

    public IActionResult Create()
    {
        // Preparar dropdown de categorías
        ViewBag.Categorias = new List<SelectListItem>
        {
            new SelectListItem { Value = "Faciales", Text = "Tratamientos Faciales" },
            new SelectListItem { Value = "Corporales", Text = "Tratamientos Corporales" },
            new SelectListItem { Value = "Masajes", Text = "Masajes" },
            new SelectListItem { Value = "Relajacion", Text = "Relajación" },
            new SelectListItem { Value = "Belleza", Text = "Belleza" }
        };
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Servicio servicio)
    {
        if (ModelState.IsValid)
        {
            // MongoDB genera automáticamente el Id
            servicio.Id = string.Empty;
            await _context.Servicios.InsertOneAsync(servicio);
            TempData["Success"] = "Servicio creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        // Recargar categorías si hay error
        ViewBag.Categorias = new List<SelectListItem>
        {
            new SelectListItem { Value = "Faciales", Text = "Tratamientos Faciales" },
            new SelectListItem { Value = "Corporales", Text = "Tratamientos Corporales" },
            new SelectListItem { Value = "Masajes", Text = "Masajes" },
            new SelectListItem { Value = "Relajacion", Text = "Relajación" },
            new SelectListItem { Value = "Belleza", Text = "Belleza" }
        };
        return View(servicio);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var servicio = await _context.Servicios
            .Find(s => s.Id == id)
            .FirstOrDefaultAsync();

        if (servicio == null)
        {
            return NotFound();
        }

        // Preparar dropdown de categorías con el tipo actual seleccionado
        ViewBag.Categorias = new List<SelectListItem>
        {
            new SelectListItem { Value = "Faciales", Text = "Tratamientos Faciales", Selected = servicio.Tipo == "Faciales" },
            new SelectListItem { Value = "Corporales", Text = "Tratamientos Corporales", Selected = servicio.Tipo == "Corporales" },
            new SelectListItem { Value = "Masajes", Text = "Masajes", Selected = servicio.Tipo == "Masajes" },
            new SelectListItem { Value = "Relajacion", Text = "Relajación", Selected = servicio.Tipo == "Relajacion" },
            new SelectListItem { Value = "Belleza", Text = "Belleza", Selected = servicio.Tipo == "Belleza" }
        };

        return View(servicio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Servicio servicio)
    {
        if (id != servicio.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var filter = Builders<Servicio>.Filter.Eq(s => s.Id, id);
            var result = await _context.Servicios.ReplaceOneAsync(filter, servicio);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            TempData["Success"] = "Servicio actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        // Recargar categorías si hay error
        ViewBag.Categorias = new List<SelectListItem>
        {
            new SelectListItem { Value = "Faciales", Text = "Tratamientos Faciales", Selected = servicio.Tipo == "Faciales" },
            new SelectListItem { Value = "Corporales", Text = "Tratamientos Corporales", Selected = servicio.Tipo == "Corporales" },
            new SelectListItem { Value = "Masajes", Text = "Masajes", Selected = servicio.Tipo == "Masajes" },
            new SelectListItem { Value = "Relajacion", Text = "Relajación", Selected = servicio.Tipo == "Relajacion" },
            new SelectListItem { Value = "Belleza", Text = "Belleza", Selected = servicio.Tipo == "Belleza" }
        };
        return View(servicio);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var servicio = await _context.Servicios
            .Find(s => s.Id == id)
            .FirstOrDefaultAsync();

        if (servicio == null)
        {
            return NotFound();
        }

        return View(servicio);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var filter = Builders<Servicio>.Filter.Eq(s => s.Id, id);
        var result = await _context.Servicios.DeleteOneAsync(filter);

        if (result.DeletedCount == 0)
        {
            TempData["Error"] = "No se pudo eliminar el servicio";
        }
        else
        {
            TempData["Success"] = "Servicio eliminado exitosamente";
        }

        return RedirectToAction(nameof(Index));
    }
}
