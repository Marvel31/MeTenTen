using MeTenTenBlazor.Models;
using System.Text.Json;

namespace MeTenTenBlazor.Services
{
    public class LocalTenTenService : ITenTenService
    {
        private readonly ILocalStorageService _localStorage;
        private const string TENTENS_KEY = "metenten_tentens";

        public LocalTenTenService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<TenTen>> GetTenTensByTopicAsync(int topicId)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY);
            return tenTens?.Where(t => t.TopicId == topicId).ToList() ?? new List<TenTen>();
        }

        public async Task<TenTen?> GetTenTenAsync(int id)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY);
            return tenTens?.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY) ?? new List<TenTen>();
            
            var newTenTen = new TenTen
            {
                Id = tenTens.Count > 0 ? tenTens.Max(t => t.Id) + 1 : 1,
                TopicId = request.TopicId,
                TopicSubject = request.TopicSubject,
                Content = request.Content,
                CreatedAt = DateTime.Now,
                UserId = 1,
                UserName = "사용자",
                IsReadByPartner = false
            };

            tenTens.Add(newTenTen);
            await _localStorage.SetItemAsync(TENTENS_KEY, tenTens);
            return newTenTen;
        }

        public async Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY) ?? new List<TenTen>();
            var tenTen = tenTens.FirstOrDefault(t => t.Id == id);
            
            if (tenTen != null)
            {
                tenTen.Content = request.Content;
                await _localStorage.SetItemAsync(TENTENS_KEY, tenTens);
            }

            return tenTen!;
        }

        public async Task DeleteTenTenAsync(int id)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY) ?? new List<TenTen>();
            tenTens.RemoveAll(t => t.Id == id);
            await _localStorage.SetItemAsync(TENTENS_KEY, tenTens);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var tenTens = await _localStorage.GetItemAsync<List<TenTen>>(TENTENS_KEY) ?? new List<TenTen>();
            var tenTen = tenTens.FirstOrDefault(t => t.Id == id);
            
            if (tenTen != null)
            {
                tenTen.IsReadByPartner = true;
                await _localStorage.SetItemAsync(TENTENS_KEY, tenTens);
            }
        }
    }
}
