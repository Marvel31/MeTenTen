using System.Net.Http.Json;
using System.Text.Json;
using MeTenTenBlazor.Models;

namespace MeTenTenBlazor.Services
{
    public class TopicService : ITopicService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TopicService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MeTenTenAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/topics");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Topic>>(content, _jsonOptions) ?? new List<Topic>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching topics: {ex.Message}");
            }
            return new List<Topic>();
        }

        public async Task<Topic?> GetTopicByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/topics/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Topic>(content, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching topic {id}: {ex.Message}");
            }
            return null;
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/topics", request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var topic = JsonSerializer.Deserialize<Topic>(content, _jsonOptions);
                    return topic ?? throw new Exception("Failed to create topic");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create topic: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating topic: {ex.Message}");
                throw;
            }
        }

        public async Task<Topic> UpdateTopicAsync(int id, UpdateTopicRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/topics/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var topic = JsonSerializer.Deserialize<Topic>(content, _jsonOptions);
                    return topic ?? throw new Exception("Failed to update topic");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update topic: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating topic {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/topics/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting topic {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleTopicStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/topics/{id}/toggle", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling topic {id}: {ex.Message}");
                return false;
            }
        }
    }
}
