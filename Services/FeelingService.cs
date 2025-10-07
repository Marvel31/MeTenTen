using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class FeelingService : IFeelingService
    {
        private readonly List<Feeling> _feelings = new();
        private int _nextId = 1;

        public FeelingService()
        {
            // 샘플 데이터 추가
            _feelings.Add(new Feeling
            {
                Id = _nextId++,
                Emoji = "😊",
                Mood = "😊 행복",
                Content = "오늘은 정말 좋은 하루였어요. 파트너와 함께 시간을 보내며 행복했습니다.",
                CreatedAt = DateTime.Now.AddDays(-1),
                UserId = 1,
                UserName = "사용자"
            });

            _feelings.Add(new Feeling
            {
                Id = _nextId++,
                Emoji = "😌",
                Mood = "😌 평온",
                Content = "평화로운 저녁 시간을 보내고 있습니다. 마음이 편안해요.",
                CreatedAt = DateTime.Now.AddHours(-3),
                UserId = 1,
                UserName = "사용자"
            });
        }

        public Task<List<Feeling>> GetAllFeelingsAsync()
        {
            return Task.FromResult(_feelings.ToList());
        }

        public Task<Feeling?> GetFeelingByIdAsync(int id)
        {
            var feeling = _feelings.FirstOrDefault(f => f.Id == id);
            return Task.FromResult(feeling);
        }

        public Task<Feeling> CreateFeelingAsync(Feeling feeling)
        {
            feeling.Id = _nextId++;
            feeling.CreatedAt = DateTime.Now;
            feeling.UserId = 1; // 임시 사용자 ID
            feeling.UserName = "사용자";
            
            _feelings.Add(feeling);
            return Task.FromResult(feeling);
        }

        public Task<Feeling> UpdateFeelingAsync(Feeling feeling)
        {
            var existingFeeling = _feelings.FirstOrDefault(f => f.Id == feeling.Id);
            if (existingFeeling != null)
            {
                existingFeeling.Emoji = feeling.Emoji;
                existingFeeling.Mood = feeling.Mood;
                existingFeeling.Content = feeling.Content;
                existingFeeling.UpdatedAt = DateTime.Now;
            }
            return Task.FromResult(existingFeeling ?? feeling);
        }

        public Task DeleteFeelingAsync(int id)
        {
            var feeling = _feelings.FirstOrDefault(f => f.Id == id);
            if (feeling != null)
            {
                _feelings.Remove(feeling);
            }
            return Task.CompletedTask;
        }

        public Task<List<Feeling>> GetFeelingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var feelings = _feelings.Where(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate).ToList();
            return Task.FromResult(feelings);
        }
    }
}

