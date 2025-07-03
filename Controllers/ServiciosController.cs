using Microsoft.AspNetCore.Mvc;
using SpaAdmin.Data;
using SpaAdmin.Models;

// Controllers/ServiciosController.cs
public class ServiciosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiciosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        var servicios = _context.Servicios
                                .OrderBy(s => s.Nombre)
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToList();
        return View(servicios);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Servicio s)
    {
        if (ModelState.IsValid)
        {
            _context.Servicios.Add(s);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(s);
    }

    public IActionResult Edit(int id)
    {
        var servicio = _context.Servicios.Find(id);
        return View(servicio);
    }

    [HttpPost]
    public IActionResult Edit(Servicio s)
    {
        if (ModelState.IsValid)
        {
            _context.Servicios.Update(s);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(s);
    }

    public IActionResult Details(int id)
    {
        var servicio = _context.Servicios.Find(id);
        return View(servicio);
    }
}
