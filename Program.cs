using MongoDB.Driver;
using SpaAdmin.Data;
using DotNetEnv;
using SpaAdmin.Services;

// Cargar variables de entorno desde el archivo .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

// Registrar ApplicationDbContext como servicio
builder.Services.AddScoped<ApplicationDbContext>();

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
