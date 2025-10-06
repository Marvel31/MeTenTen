using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class TenTenService : ITenTenService
    {
        private static List<TenTen> _tenTens = new();
        private static int _nextId = 1;

        public TenTenService()
        {
            // 샘플 데이터 추가
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
            }
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

        public Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
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
            return Task.FromResult(tenTen);
        }

        public Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                throw new ArgumentException($"TenTen with ID {id} not found");

            tenTen.Content = request.Content;
            tenTen.UpdatedAt = DateTime.Now;
            tenTen.EmotionTag = request.EmotionTag;
            tenTen.ImportanceLevel = request.ImportanceLevel;

            return Task.FromResult(tenTen);
        }

        public Task<TenTen> UpdateTenTenAsync(TenTen tenTen)
        {
            var existingTenTen = _tenTens.FirstOrDefault(t => t.Id == tenTen.Id);
            if (existingTenTen == null)
                throw new ArgumentException($"TenTen with ID {tenTen.Id} not found");

            existingTenTen.Content = tenTen.Content;
            existingTenTen.UpdatedAt = DateTime.Now;
            existingTenTen.EmotionTag = tenTen.EmotionTag;
            existingTenTen.ImportanceLevel = tenTen.ImportanceLevel;

            return Task.FromResult(existingTenTen);
        }

        public Task<bool> DeleteTenTenAsync(int id)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                return Task.FromResult(false);

            _tenTens.Remove(tenTen);
            return Task.FromResult(true);
        }

        public Task<bool> MarkAsReadAsync(int id)
        {
            var tenTen = _tenTens.FirstOrDefault(t => t.Id == id);
            if (tenTen == null)
                return Task.FromResult(false);

            tenTen.IsReadByPartner = true;
            tenTen.ReadByPartnerAt = DateTime.Now;
            return Task.FromResult(true);
        }
    }
}

