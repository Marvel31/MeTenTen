using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class TopicService : ITopicService
    {
        private static List<Topic> _topics = new();
        private static int _nextId = 1;
        private static bool _isInitialized = false;
        private readonly IFileStorageService _fileStorageService;

        public TopicService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
            
            // 싱글톤이므로 한 번만 초기화
            if (!_isInitialized)
            {
                _isInitialized = true;
                LoadDataFromFile().Wait();
                
                // 데이터가 없으면 샘플 데이터 추가
                if (!_topics.Any())
                {
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
                    
                    SaveDataToFile().Wait();
                }
            }
        }

        private async Task LoadDataFromFile()
        {
            var loadedData = await _fileStorageService.LoadTopicsAsync();
            if (loadedData.Any())
            {
                _topics = loadedData;
                _nextId = _topics.Max(t => t.Id) + 1;
            }
        }

        private async Task SaveDataToFile()
        {
            await _fileStorageService.SaveTopicsAsync(_topics);
        }

        public Task<List<Topic>> GetTopicsAsync()
        {
            return Task.FromResult(_topics.Where(t => t.IsActive).ToList());
        }

        public Task<List<Topic>> GetAllTopicsAsync()
        {
            return Task.FromResult(_topics.ToList());
        }

        public Task<Topic?> GetTopicByIdAsync(int id)
        {
            var result = _topics.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(result);
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
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
            var topic = _topics.FirstOrDefault(t => t.Id == id);
            if (topic == null)
                return false;

            topic.IsActive = false; // 소프트 삭제
            await SaveDataToFile();
            return true;
        }
    }
}

