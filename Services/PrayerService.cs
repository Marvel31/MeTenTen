using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class PrayerService : IPrayerService
    {
        private readonly List<Prayer> _prayers = new();
        private int _nextId = 1;

        public PrayerService()
        {
            // 샘플 기도문 데이터 추가
            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "부부를 위한 기도",
                Content = "하나님, 저희 부부가 서로를 사랑하고 존중하며, 함께 성장할 수 있도록 도와주세요. 어려운 시간에도 서로를 지지하고 격려할 수 있는 힘을 주시고, 평생을 함께 걸어갈 수 있는 믿음과 사랑을 주시옵소서. 아멘.",
                Category = "marriage",
                Tags = "부부, 사랑, 결혼, 가정",
                CreatedAt = DateTime.Now.AddDays(-10),
                IsFavorite = true,
                ViewCount = 15
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "가족의 평화를 위한 기도",
                Content = "하나님, 저희 가정에 평화와 화합이 있도록 도와주세요. 가족 구성원들이 서로를 이해하고 사랑하며, 어려운 일이 있을 때도 함께 극복할 수 있는 지혜와 힘을 주시옵소서. 저희 가정이 하나님의 사랑으로 가득한 곳이 되도록 인도해주세요. 아멘.",
                Category = "family",
                Tags = "가족, 평화, 화합, 가정",
                CreatedAt = DateTime.Now.AddDays(-8),
                IsFavorite = false,
                ViewCount = 8
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "하루를 시작하며 드리는 기도",
                Content = "하나님, 오늘 하루를 주신 것에 감사드립니다. 오늘도 주님의 뜻에 따라 살아갈 수 있도록 도와주시고, 저희에게 주어진 모든 것에 감사하며 살 수 있도록 마음을 열어주세요. 어려운 일이 있어도 주님의 은혜로 극복할 수 있도록 도와주시옵소서. 아멘.",
                Category = "daily",
                Tags = "일상, 감사, 시작, 은혜",
                CreatedAt = DateTime.Now.AddDays(-5),
                IsFavorite = true,
                ViewCount = 12
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "부부 소통을 위한 기도",
                Content = "하나님, 저희 부부가 서로의 마음을 이해하고 소통할 수 있도록 도와주세요. 말로 표현하기 어려운 마음도 서로 알아차릴 수 있는 지혜를 주시고, 서로를 향한 사랑과 관심이 더욱 깊어지도록 도와주시옵소서. 저희의 대화가 서로를 더 가깝게 만드는 소중한 시간이 되도록 인도해주세요. 아멘.",
                Category = "marriage",
                Tags = "부부, 소통, 이해, 대화",
                CreatedAt = DateTime.Now.AddDays(-3),
                IsFavorite = false,
                ViewCount = 6
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "하루를 마무리하며 드리는 기도",
                Content = "하나님, 오늘 하루를 무사히 마무리할 수 있게 해주셔서 감사합니다. 오늘 하루 동안 주신 모든 은혜와 축복에 감사드리며, 내일도 주님의 뜻에 따라 살아갈 수 있도록 도와주시옵소서. 저희 가족 모두가 평안한 밤을 보내고, 내일도 건강하고 행복한 하루를 시작할 수 있도록 도와주세요. 아멘.",
                Category = "daily",
                Tags = "일상, 마무리, 감사, 평안",
                CreatedAt = DateTime.Now.AddDays(-1),
                IsFavorite = false,
                ViewCount = 4
            });
        }

        public Task<List<Prayer>> GetAllPrayersAsync()
        {
            return Task.FromResult(_prayers.ToList());
        }

        public Task<Prayer?> GetPrayerByIdAsync(int id)
        {
            var prayer = _prayers.FirstOrDefault(p => p.Id == id);
            if (prayer != null)
            {
                prayer.ViewCount++;
            }
            return Task.FromResult(prayer);
        }

        public Task<List<Prayer>> GetPrayersByCategoryAsync(string category)
        {
            var prayers = _prayers.Where(p => p.Category == category).ToList();
            return Task.FromResult(prayers);
        }

        public Task<List<Prayer>> GetFavoritePrayersAsync()
        {
            var prayers = _prayers.Where(p => p.IsFavorite).ToList();
            return Task.FromResult(prayers);
        }

        public Task<Prayer> CreatePrayerAsync(Prayer prayer)
        {
            prayer.Id = _nextId++;
            prayer.CreatedAt = DateTime.Now;
            prayer.ViewCount = 0;
            
            _prayers.Add(prayer);
            return Task.FromResult(prayer);
        }

        public Task<Prayer> UpdatePrayerAsync(Prayer prayer)
        {
            var existingPrayer = _prayers.FirstOrDefault(p => p.Id == prayer.Id);
            if (existingPrayer != null)
            {
                existingPrayer.Title = prayer.Title;
                existingPrayer.Content = prayer.Content;
                existingPrayer.Category = prayer.Category;
                existingPrayer.Tags = prayer.Tags;
                existingPrayer.UpdatedAt = DateTime.Now;
            }
            return Task.FromResult(existingPrayer ?? prayer);
        }

        public Task DeletePrayerAsync(int id)
        {
            var prayer = _prayers.FirstOrDefault(p => p.Id == id);
            if (prayer != null)
            {
                _prayers.Remove(prayer);
            }
            return Task.CompletedTask;
        }

        public Task ToggleFavoriteAsync(int id)
        {
            var prayer = _prayers.FirstOrDefault(p => p.Id == id);
            if (prayer != null)
            {
                prayer.IsFavorite = !prayer.IsFavorite;
            }
            return Task.CompletedTask;
        }
    }
}

