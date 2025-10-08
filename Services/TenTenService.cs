using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class TenTenService : ITenTenService
    {
        private static List<TenTen> _tenTens = new();
        private static int _nextId = 1;
        private static bool _isInitialized = false;
        private readonly IFileStorageService _fileStorageService;

        public TenTenService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
            
            // 싱글톤이므로 한 번만 초기화
            if (!_isInitialized)
            {
                _isInitialized = true;
                LoadDataFromFile().Wait();
                
                // 데이터가 없으면 샘플 데이터 추가
                if (!_tenTens.Any())
                {
                    _tenTens.Add(new TenTen
                    {
                        Id = _nextId++,
                        Content = "오늘 하루도 고마웠어요. 항상 나를 이해해주고 사랑해주셔서 감사합니다.",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        UserId = 1,
                        UserName = "사용자1",
                        TopicId = 1,
                        TopicSubject = "감사한 마음",
                        IsReadByPartner = true,
                        ReadByPartnerAt = DateTime.Now.AddDays(-1).AddHours(2)
                    });

                    _tenTens.Add(new TenTen
                    {
                        Id = _nextId++,
                        Content = "오늘은 조금 피곤했지만, 당신과 함께라서 힘이 났어요. 내일도 함께해요.",
                        CreatedAt = DateTime.Now.AddHours(-3),
                        UserId = 2,
                        UserName = "사용자2",
                        TopicId = 1,
                        TopicSubject = "감사한 마음",
                        IsReadByPartner = false
                    });
                    
                    SaveDataToFile().Wait();
                }
            }
        }

        private async Task LoadDataFromFile()
        {
            var loadedData = await _fileStorageService.LoadTenTensAsync();
            if (loadedData.Any())
            {
                _tenTens = loadedData;
                _nextId = _tenTens.Max(t => t.Id) + 1;
            }
        }

        private async Task SaveDataToFile()
        {
            await _fileStorageService.SaveTenTensAsync(_tenTens);
        }

        public Task<List<TenTen>> GetTenTensAsync()
        {
            return Task.FromResult(_tenTens.ToList());
        }

        public Task<List<TenTen>> GetAllTenTensAsync()
        {
            return Task.FromResult(_tenTens.ToList());
        }

        public Task<List<TenTen>> GetTenTensByTopicAsync(int topicId)
        {
            var result = _tenTens.Where(t => t.TopicId == topicId).ToList();
            return Task.FromResult(result);
        }

        public Task<TenTen?> GetTenTenByIdAsync(int id)
        {
            var result = _tenTens.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(result);
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
        {
            var tenTen = new TenTen
            {
                Id = _nextId++,
                Content = request.Content,
                CreatedAt = DateTime.Now,
                UserId = 1, // 현재 사용자 ID (실제로는 인증된 사용자 ID 사용)
                UserName = "현재 사용자",
                TopicId = request.TopicId,
                TopicSubject = "새 주제", // 실제로는 Topic 서비스에서 가져와야 함
                IsReadByPartner = false,
                EmotionTag = request.EmotionTag,
                ImportanceLevel = request.ImportanceLevel
            };

            _tenTens.Add(tenTen);
            await SaveDataToFile();
            return tenTen;
        }

        public async Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                throw new ArgumentException($"TenTen with ID {id} not found");

            tenTen.Content = request.Content;
            tenTen.UpdatedAt = DateTime.Now;
            tenTen.EmotionTag = request.EmotionTag;
            tenTen.ImportanceLevel = request.ImportanceLevel;

            await SaveDataToFile();
            return tenTen;
        }

        public async Task<TenTen> UpdateTenTenAsync(TenTen tenTen)
        {
            var existingTenTen = _tenTens.FirstOrDefault(t => t.Id == tenTen.Id);
            if (existingTenTen == null)
                throw new ArgumentException($"TenTen with ID {tenTen.Id} not found");

            existingTenTen.Content = tenTen.Content;
            existingTenTen.UpdatedAt = DateTime.Now;
            existingTenTen.EmotionTag = tenTen.EmotionTag;
            existingTenTen.ImportanceLevel = tenTen.ImportanceLevel;

            await SaveDataToFile();
            return existingTenTen;
        }

        public async Task<bool> DeleteTenTenAsync(int id)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                return false;

            _tenTens.Remove(tenTen);
            await SaveDataToFile();
            return true;
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                return false;

            tenTen.IsReadByPartner = true;
            tenTen.ReadByPartnerAt = DateTime.Now;
            await SaveDataToFile();
            return true;
        }
    }
}

