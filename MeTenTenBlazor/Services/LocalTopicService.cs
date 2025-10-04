using MeTenTenBlazor.Models;
using System.Text.Json;

namespace MeTenTenBlazor.Services
{
    public class LocalTopicService : ITopicService
    {
        private readonly ILocalStorageService _localStorage;
        private const string TOPICS_KEY = "metenten_topics";

        public LocalTopicService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            var topics = await _localStorage.GetItemAsync<List<Topic>>(TOPICS_KEY);
            return topics ?? new List<Topic>();
        }

        public async Task<Topic?> GetTopicAsync(int id)
        {
            var topics = await GetTopicsAsync();
            return topics.FirstOrDefault(t => t.Id == id);
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
            var topics = await GetTopicsAsync();
            var newTopic = new Topic
            {
                Id = topics.Count > 0 ? topics.Max(t => t.Id) + 1 : 1,
                Subject = request.Subject,
                TopicDate = request.TopicDate,
                CreatedAt = DateTime.Now,
                IsActive = true,
                CreatedByUserId = 1,
                CreatedByUserName = "사용자"
            };

            topics.Add(newTopic);
            await _localStorage.SetItemAsync(TOPICS_KEY, topics);
            return newTopic;
        }

        public async Task<Topic> UpdateTopicAsync(int id, UpdateTopicRequest request)
        {
            var topics = await GetTopicsAsync();
            var topic = topics.FirstOrDefault(t => t.Id == id);
            
            if (topic != null)
            {
                topic.Subject = request.Subject;
                topic.TopicDate = request.TopicDate;
                topic.IsActive = request.IsActive;
                await _localStorage.SetItemAsync(TOPICS_KEY, topics);
            }

            return topic!;
        }

        public async Task DeleteTopicAsync(int id)
        {
            var topics = await GetTopicsAsync();
            topics.RemoveAll(t => t.Id == id);
            await _localStorage.SetItemAsync(TOPICS_KEY, topics);
        }
    }
}
