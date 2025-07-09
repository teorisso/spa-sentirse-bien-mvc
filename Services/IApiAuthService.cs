using SpaAdmin.Models;

namespace SpaAdmin.Services
{
    public interface IApiAuthService
    {
        Task<bool> LoginAsync();          // Realiza el login y guarda el token
        string? Token { get; }            // JWT obtenido
        DateTime? TokenExpiry { get; }    // Expiración
    }
}