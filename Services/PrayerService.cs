using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class PrayerService : IPrayerService
    {
        private readonly List<Prayer> _prayers = new();
        private int _nextId = 1;

        public PrayerService()
        {
            // 주요 기도문 데이터 추가
            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "부부의 기도",
                Content = @"○ 인자하신 하느님 아버지,
    혼인성사로 저희를 맺어 주시고
    보살펴 주시니 감사하나이다.
● 이제 저희가 혼인 서약을 되새기며 청하오니
    저희 부부가 그 서약을 따라
    즐거울 때나 괴로울 때나
    잘살 때나 못살 때나
    성할 때나 아플 때나
    서로 사랑하고 존경하며 신의를 지키게 하소서.
○ 또 청하오니
    언제나 주님을 찬미하는 저희 부부의 삶이
    주님의 사랑을 드러내는 성사가 되게 하소서.
    우리 주 그리스도를 통하여 비나이다.
◎ 아멘.",
                Category = "marriage",
                Tags = "부부, 혼인성사, 서약, 사랑",
                CreatedAt = DateTime.Now.AddDays(-10),
                IsFavorite = true,
                ViewCount = 0
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "가정을 위한 기도",
                Content = @"○ 마리아와 요셉에게 순종하시며
    가정생활을 거룩하게 하신 예수님,
    저희 가정을 거룩하게 하시고
    저희가 성가정을 본받아
    주님의 뜻을 따라 살게 하소서.
● 가정생활의 자랑이며 모범이신
    성모 마리아와 성 요셉,
    저희 집안을 위하여 빌어 주시어
    모든 가족이 건강하고 행복하게 하시며
    언제나 주님을 섬기고 이웃을 사랑하며 살다가
    주님의 은총으로 영원한 천상 가정에 들게 하소서.
◎ 아멘.",
                Category = "family",
                Tags = "가정, 성가정, 성모마리아, 성요셉",
                CreatedAt = DateTime.Now.AddDays(-9),
                IsFavorite = true,
                ViewCount = 0
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "자녀를 위한 기도",
                Content = @"○ 세상을 창조하신 하느님,
    하느님께서는 저희에게 귀한 자녀를 주시어
    창조를 이어가게 하셨으니
    주님의 사랑으로 자녀를 길러
    주님의 영광을 드러내게 하소서.
● 주님, 사랑하는 저희 자녀를
    은총으로 보호하시어
    세상 부패에 물들지 않게 하시며
    온갖 악의 유혹을 물리치고
    예수님을 본받아
    주님의 뜻을 이루는 일꾼이 되게 하소서.
    우리 주 그리스도를 통하여 비나이다.
◎ 아멘.",
                Category = "family",
                Tags = "자녀, 보호, 은총, 양육",
                CreatedAt = DateTime.Now.AddDays(-8),
                IsFavorite = false,
                ViewCount = 0
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "부모를 위한 기도",
                Content = @"○ 인자하신 하느님,
    하느님께서는 부모를 사랑하고 공경하며
    그 은덕에 감사하라 하셨으니
    저희가 효성을 다하여 부모를 섬기겠나이다.
● 저희 부모는 저희를 낳아 기르며
    갖은 어려움을 기쁘게 이겨 냈으니
    이제는 그 보람을 느끼며
    편히 지내게 하소서.
○ 주님, 저희 부모에게 강복하시고
    은총으로 지켜 주시며
    마침내 영원한 행복을 누리게 하소서.
    우리 주 그리스도를 통하여 비나이다.
◎ 아멘.",
                Category = "family",
                Tags = "부모, 효도, 공경, 은덕",
                CreatedAt = DateTime.Now.AddDays(-7),
                IsFavorite = false,
                ViewCount = 0
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "사제들을 위한 기도",
                Content = @"○ 영원한 사제이신 예수님,
    주님을 본받으려는 사제들을 지켜 주시어
    어느 누구도 그들을 해치지 못하게 하소서.
● 주님의 영광스러운 사제직에 올라
    날마다 주님의 몸과 피를 축성하는 사제들을
    언제나 깨끗하고 거룩하게 지켜 주소서.
○ 주님의 뜨거운 사랑으로
    사제들을 세속에 물들지 않도록 지켜 주소서.
● 사제들이 하는 모든 일에 강복하시어
    은총의 풍부한 열매를 맺게 하시고
○ 저희로 말미암아
    세상에서는 그들이 더없는 기쁨과 위안을 얻고
    천국에서는 찬란히 빛나는
    영광을 누리게 하소서.
◎ 아멘.",
                Category = "church",
                Tags = "사제, 성직자, 거룩함, 봉헌",
                CreatedAt = DateTime.Now.AddDays(-6),
                IsFavorite = false,
                ViewCount = 0
            });

            _prayers.Add(new Prayer
            {
                Id = _nextId++,
                Title = "수도자들을 위한 기도",
                Content = @"○ 세례성사의 은총을 더욱 풍부하게 열매 맺도록
    자녀들을 수도자의 길로 부르시는 하느님,
    수도자들을 통하여
    끊임없이 하느님을 찾고
    오롯한 사랑으로 그리스도께 봉헌하는 삶이
    교회의 시작부터 지금까지 지속되게 하시니 감사하나이다.
● 하느님,
    수도자들이 성령께 온전히 귀 기울여
    복음의 증거자로서 정결과 청빈과 순명의 삶을 살게 하시어
    자유로이 그리스도를 따르고 더욱 그리스도를 닮아
    세상의 구원을 위하여 기도하고 봉사하게 하소서.
    우리 주 그리스도를 통하여 비나이다.
◎ 아멘.",
                Category = "church",
                Tags = "수도자, 봉헌, 정결, 청빈, 순명",
                CreatedAt = DateTime.Now.AddDays(-5),
                IsFavorite = false,
                ViewCount = 0
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

