using System.Net.Http.Headers;
using System.Net.Http.Json;
using SpaAdmin.Models;

namespace SpaAdmin.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient      _http;
        private readonly IApiAuthService _auth;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient http,
                         IApiAuthService auth,
                         ILogger<ApiClient> logger)
        {
            _http   = http;
            _auth   = auth;
            _logger = logger;
        }

        // Garantiza que haya token y lo adjunta al header
        private async Task EnsureTokenAsync()
        {
            if (_auth.Token is null || _auth.TokenExpiry <= DateTime.UtcNow)
                await _auth.LoginAsync();

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _auth.Token);
        }

        public async Task<T?> GetAsync<T>(string url) where T : class
        {
            await EnsureTokenAsync();
            var resp = await _http.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET {Url} devolvió {Status}", url, resp.StatusCode);
                return default;
            }
            var apiResp = await resp.Content.ReadFromJsonAsync<ApiResponse<T>>();
            return apiResp?.Data;
        }

        public async Task<TResult?> PostAsync<TBody, TResult>(string url, TBody body) where TResult : class
        {
            await EnsureTokenAsync();
            var resp = await _http.PostAsJsonAsync(url, body);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("POST {Url} devolvió {Status}", url, resp.StatusCode);
                return default;
            }
            var apiResp = await resp.Content.ReadFromJsonAsync<ApiResponse<TResult>>();
            return apiResp?.Data;
        }

        public async Task<bool> PutAsync<TBody>(string url, TBody body)
        {
            await EnsureTokenAsync();
            var resp = await _http.PutAsJsonAsync(url, body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            await EnsureTokenAsync();
            var resp = await _http.DeleteAsync(url);
            return resp.IsSuccessStatusCode;
        }
        public async Task<T?> GetDirectAsync<T>(string url) where T : class
        {
            await EnsureTokenAsync();
            var resp = await _http.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET {Url} devolvió {Status}", url, resp.StatusCode);
                return default;
            }

            return await resp.Content.ReadFromJsonAsync<T>();
        }
    }
}