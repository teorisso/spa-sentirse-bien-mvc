using MongoDB.Driver;
using SpaAdmin.Data;
using DotNetEnv;
using SpaAdmin.Services;
using Microsoft.AspNetCore.Diagnostics;

// Cargar variables de entorno desde el archivo .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configurar MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Registrar ApplicationDbContext como servicio
builder.Services.AddScoped<ApplicationDbContext>();

// Registrar servicios de validación
builder.Services.AddScoped<IValidationService, ValidationService>();

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Configuración de autenticación con cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
    });

// Configurar HttpClient para ApiAuthService con BaseAddress
builder.Services.AddHttpClient<IApiAuthService, ApiAuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5018/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configurar HttpClient para ApiClient con BaseAddress
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5018/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // En desarrollo, mostrar errores detallados
    app.UseDeveloperExceptionPage();
}

// Middleware personalizado para manejo de errores
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
        
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            context.Response.Redirect("/Home/Error");
        }
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
