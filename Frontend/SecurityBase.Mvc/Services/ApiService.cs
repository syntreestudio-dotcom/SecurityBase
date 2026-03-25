using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SecurityBase.Mvc.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T, U>(string endpoint, U data);
    Task<T?> PutAsync<T, U>(string endpoint, U data);
    Task<T?> DeleteAsync<T>(string endpoint);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _baseUrl;

    public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api/";
    }

    private void AddAuthHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        AddAuthHeader();
        var response = await _httpClient.GetAsync(_baseUrl + endpoint);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return default;
    }

    public async Task<T?> PostAsync<T, U>(string endpoint, U data)
    {
        AddAuthHeader();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return default;
    }

    public async Task<T?> PutAsync<T, U>(string endpoint, U data)
    {
        AddAuthHeader();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return default;
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync(_baseUrl + endpoint);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return default;
    }
}
