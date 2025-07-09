using System.Net.Http.Json;

namespace SpaAdmin.Services
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string url) where T : class;
        Task<TResult?> PostAsync<TBody, TResult>(string url, TBody body) where TResult : class;
        Task<bool> PutAsync<TBody>(string url, TBody body);
        Task<TResult?> PutAsync<TBody, TResult>(string url, TBody body) where TResult : class;
        Task<bool> DeleteAsync(string url);
        Task<T?> GetDirectAsync<T>(string url) where T : class;
    }
}