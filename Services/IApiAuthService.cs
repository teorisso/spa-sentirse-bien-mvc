using SpaAdmin.Models;

namespace SpaAdmin.Services
{
    public interface IApiAuthService
    {
        // Autenticación interna del sistema (configurada en appsettings)
        Task<bool> LoginAsync();          // Realiza el login y guarda el token
        string? Token { get; }            // JWT obtenido
        DateTime? TokenExpiry { get; }    // Expiración

        // Autenticación de usuarios administrativos
        Task<ApiResponse<AuthResponse>?> LoginUserAsync(LoginDto loginDto);
        Task<ApiResponse<AuthResponse>?> RegisterUserAsync(RegisterDto registerDto);
        Task<ApiResponse<string>?> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ApiResponse<string>?> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}