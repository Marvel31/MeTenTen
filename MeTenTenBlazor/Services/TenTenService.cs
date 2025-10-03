using System.Net.Http.Json;
using System.Text.Json;
using MeTenTenBlazor.Models;

namespace MeTenTenBlazor.Services
{
    public class TenTenService : ITenTenService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TenTenService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MeTenTenAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<TenTen>> GetTenTensAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/diaries");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TenTen>>(content, _jsonOptions) ?? new List<TenTen>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching 10&10s: {ex.Message}");
            }
            return new List<TenTen>();
        }

        public async Task<List<TenTen>> GetTenTensByTopicAsync(int topicId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/diaries/topic/{topicId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TenTen>>(content, _jsonOptions) ?? new List<TenTen>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching 10&10s for topic {topicId}: {ex.Message}");
            }
            return new List<TenTen>();
        }

        public async Task<TenTen?> GetTenTenByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/diaries/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TenTen>(content, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching 10&10 {id}: {ex.Message}");
            }
            return null;
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/diaries", request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tenTen = JsonSerializer.Deserialize<TenTen>(content, _jsonOptions);
                    return tenTen ?? throw new Exception("Failed to create 10&10");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create 10&10: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating 10&10: {ex.Message}");
                throw;
            }
        }

        public async Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/diaries/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tenTen = JsonSerializer.Deserialize<TenTen>(content, _jsonOptions);
                    return tenTen ?? throw new Exception("Failed to update 10&10");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update 10&10: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating 10&10 {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTenTenAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/diaries/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting 10&10 {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/diaries/{id}/mark-read", new { DiaryId = id });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking 10&10 {id} as read: {ex.Message}");
                return false;
            }
        }
    }
}
