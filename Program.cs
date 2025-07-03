using Microsoft.EntityFrameworkCore;
using SpaAdmin.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar DbContext con cadena de conexión
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Resto de la configuración...
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware...
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
