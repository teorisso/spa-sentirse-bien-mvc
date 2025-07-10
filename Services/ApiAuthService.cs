using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using SpaAdmin.Models;

namespace SpaAdmin.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<ApiAuthService> _logger;

        public string? Token { get; private set; }
        public DateTime? TokenExpiry { get; private set; }

        public ApiAuthService(HttpClient httpClient,
                              IConfiguration config,
                              ILogger<ApiAuthService> logger)
        {
            _httpClient = httpClient;
            _config     = config;
            _logger     = logger;
        }

        public async Task<bool> LoginAsync()
        {
            // Credenciales y endpoint desde appsettings / .env
            var email    = _config["ApiAuth:Email"];
            var password = _config["ApiAuth:Password"];
            var endpoint = $"{_config["ApiSettings:AuthEndpoint"]}/login";

            var loginDto = new LoginDto { Email = email!, Password = password! };

            var response = await _httpClient.PostAsJsonAsync(endpoint, loginDto);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Login API falló. Status: {Status}", response.StatusCode);
                return false;
            }

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

            if (apiResponse is null || !apiResponse.Success || apiResponse.Data is null)
            {
                _logger.LogError("Estructura de respuesta inesperada al loguear.");
                return false;
            }

            Token       = apiResponse.Data.Token;
            TokenExpiry = apiResponse.Data.ExpiresAt;

            // Adjuntar el JWT para próximas llamadas
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Token);

            _logger.LogInformation("Login API exitoso – token guardado (vence {Expiry})", TokenExpiry);
            return true;
        }

        public async Task<ApiResponse<AuthResponse>?> LoginUserAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Login de usuario falló. Status: {Status}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error content: {Content}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
                _logger.LogInformation("Login de usuario exitoso para: {Email}", loginDto.Email);
                
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al hacer login de usuario: {Email}", loginDto.Email);
                return null;
            }
        }

        public async Task<ApiResponse<AuthResponse>?> RegisterUserAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Registro de usuario falló. Status: {Status}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error content: {Content}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
                _logger.LogInformation("Registro de usuario exitoso para: {Email}", registerDto.Email);
                
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario: {Email}", registerDto.Email);
                return null;
            }
        }

        public async Task<ApiResponse<string>?> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", forgotPasswordDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Recuperación de contraseña falló. Status: {Status}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error content: {Content}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                _logger.LogInformation("Solicitud de recuperación exitosa para: {Email}", forgotPasswordDto.Email);
                
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al solicitar recuperación de contraseña: {Email}", forgotPasswordDto.Email);
                return null;
            }
        }

        public async Task<ApiResponse<string>?> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/reset-password", resetPasswordDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Reset de contraseña falló. Status: {Status}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error content: {Content}", errorContent);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                _logger.LogInformation("Reset de contraseña exitoso");
                
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear contraseña");
                return null;
            }
        }
    }
}