using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class FirebaseTopicService : ITopicService
    {
        private readonly IFirebaseDataService _firebaseDataService;
        private readonly IAuthService _authService;
        private List<Topic> _cachedTopics = new();
        private int _nextId = 1;

        public FirebaseTopicService(IFirebaseDataService firebaseDataService, IAuthService authService)
        {
            _firebaseDataService = firebaseDataService;
            _authService = authService;
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                System.Diagnostics.Debug.WriteLine("[FirebaseTopicService] User not authenticated");
                return new List<Topic>();
            }

            _cachedTopics = await _firebaseDataService.GetTopicsAsync(_authService.CurrentUserId);
            
            // ID 재할당
            _nextId = 1;
            foreach (var topic in _cachedTopics.OrderBy(t => t.CreatedAt))
            {
                topic.Id = _nextId++;
            }

            return _cachedTopics.Where(t => t.IsActive).ToList();
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return new List<Topic>();
            }

            _cachedTopics = await _firebaseDataService.GetTopicsAsync(_authService.CurrentUserId);
            
            // ID 재할당
            _nextId = 1;
            foreach (var topic in _cachedTopics.OrderBy(t => t.CreatedAt))
            {
                topic.Id = _nextId++;
            }

            return _cachedTopics;
        }

        public async Task<Topic?> GetTopicByIdAsync(int id)
        {
            var topics = await GetAllTopicsAsync();
            return topics.FirstOrDefault(t => t.Id == id);
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var topic = await _firebaseDataService.CreateTopicAsync(_authService.CurrentUserId, request);
            return topic;
        }

        public async Task<Topic> UpdateTopicAsync(int id, UpdateTopicRequest request)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // 로컬 캐시에서 Firebase Key 찾기
            var existingTopic = _cachedTopics.FirstOrDefault(t => t.Id == id);
            if (existingTopic == null || string.IsNullOrEmpty(existingTopic.FirebaseKey))
            {
                throw new ArgumentException($"Topic with ID {id} not found");
            }

            var topic = await _firebaseDataService.UpdateTopicAsync(_authService.CurrentUserId, existingTopic.FirebaseKey, request);
            return topic;
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return false;
            }

            var existingTopic = _cachedTopics.FirstOrDefault(t => t.Id == id);
            if (existingTopic == null || string.IsNullOrEmpty(existingTopic.FirebaseKey))
            {
                return false;
            }

            return await _firebaseDataService.DeleteTopicAsync(_authService.CurrentUserId, existingTopic.FirebaseKey);
        }
    }
}

