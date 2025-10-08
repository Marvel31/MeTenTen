using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class TopicService : ITopicService
    {
        private static List<Topic> _topics = new();
        private static int _nextId = 1;
        private static bool _isInitialized = false;
        private static Task? _initializationTask = null;
        private static readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private readonly IFileStorageService _fileStorageService;

        public TopicService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                if (_initializationTask == null)
                {
                    _initializationTask = InitializeAsync();
                }

                await _initializationTask;
                _isInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task InitializeAsync()
        {
            var loadedData = await _fileStorageService.LoadTopicsAsync();
            if (loadedData.Any())
            {
                _topics = loadedData;
                _nextId = _topics.Max(t => t.Id) + 1;
            }
            else
            {
                // 데이터가 없으면 샘플 데이터 추가
                _topics.Add(new Topic
                {
                    Id = _nextId++,
                    Subject = "감사한 마음",
                    Description = "오늘 하루 동안 감사했던 것들에 대해 이야기해보세요.",
                    TopicDate = DateTime.Today.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-1),
                    IsActive = true
                });

                _topics.Add(new Topic
                {
                    Id = _nextId++,
                    Subject = "서로의 꿈",
                    Description = "각자의 꿈과 목표에 대해 이야기해보세요.",
                    TopicDate = DateTime.Today,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                });

                _topics.Add(new Topic
                {
                    Id = _nextId++,
                    Subject = "가족의 소중함",
                    Description = "가족의 소중함에 대해 생각해보고 이야기해보세요.",
                    TopicDate = DateTime.Today.AddDays(1),
                    CreatedAt = DateTime.Now.AddDays(1),
                    IsActive = true
                });
                
                await SaveDataToFile();
            }
        }

        private async Task SaveDataToFile()
        {
            await _fileStorageService.SaveTopicsAsync(_topics);
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            await EnsureInitializedAsync();
            return _topics.Where(t => t.IsActive).ToList();
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            await EnsureInitializedAsync();
            return _topics.ToList();
        }

        public async Task<Topic?> GetTopicByIdAsync(int id)
        {
            await EnsureInitializedAsync();
            var result = _topics.FirstOrDefault(t => t.Id == id);
            return result;
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
            await EnsureInitializedAsync();
            
            var topic = new Topic
            {
                Id = _nextId++,
                Subject = request.Subject,
                Description = request.Description,
                TopicDate = request.TopicDate,
                CreatedAt = request.CreatedAt,
                IsActive = true
            };

            _topics.Add(topic);
            await SaveDataToFile();
            return topic;
        }

        public async Task<Topic> UpdateTopicAsync(int id, UpdateTopicRequest request)
        {
            await EnsureInitializedAsync();
            
            var topic = _topics.FirstOrDefault(t => t.Id == id);
            if (topic == null)
                throw new ArgumentException($"Topic with ID {id} not found");

            topic.Subject = request.Subject;
            topic.Description = request.Description;
            topic.TopicDate = request.TopicDate;
            topic.UpdatedAt = DateTime.Now;
            topic.IsActive = request.IsActive;

            await SaveDataToFile();
            return topic;
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            await EnsureInitializedAsync();
            
            var topic = _topics.FirstOrDefault(t => t.Id == id);
            if (topic == null)
                return false;

            topic.IsActive = false; // 소프트 삭제
            await SaveDataToFile();
            return true;
        }
    }
}


