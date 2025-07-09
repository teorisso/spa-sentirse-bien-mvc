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
    }
}