using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class FeelingExampleService : IFeelingExampleService
    {
        private static List<FeelingExample> _examples = new();
        private static int _nextId = 1;
        private static bool _isInitialized = false;
        private static Task? _initializationTask = null;
        private static readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private readonly IFileStorageService _fileStorageService;

        public FeelingExampleService(IFileStorageService fileStorageService)
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
            var loadedData = await _fileStorageService.LoadFeelingExamplesAsync();
            if (loadedData.Any())
            {
                _examples = loadedData;
                _nextId = _examples.Max(e => e.Id) + 1;
            }
            else
            {
                // 기본 예시 데이터 초기화
                InitializeDefaultExamples();
                await SaveDataToFile();
            }
        }

        private void InitializeDefaultExamples()
        {
            // 기쁨 카테고리
            AddDefaultExample("기쁨", "가벼운", "새털처럼 빈 기방을 들 때처럼, 힘든 숙제를 다 마쳤을 때처럼");
            AddDefaultExample("기쁨", "개운한", "목욕한 다음처럼, 고해성사를 보았을 때처럼");
            AddDefaultExample("기쁨", "감격스런", "선생님께 뜻하지 않게 칭찬을 들었을 때처럼, 올림픽 시상식에서 태극기가 올라가고 애국가가 울려 펴질 때처럼");
            AddDefaultExample("기쁨", "감동을받은", "원하던 선물을 배우자에게 받았을 때처럼, 뜻밖에도 말썽꾸러기 아들로부터 사랑한다는 편지를 받았을 때처럼");
            AddDefaultExample("기쁨", "경이로운", "바다 속 신비의 세계를 보았을 때처럼, 첫 출산 후 아이를 볼 때처럼");
            AddDefaultExample("기쁨", "경쾌한", "아침에 일어나 행진곡을 들을 때처럼, 리듬 체조를 하는 여자 선수의 동작을 볼 때처럼");
            AddDefaultExample("기쁨", "고마운", "길을 친절하게 안내 받았을 때처럼, 무거운 짐을 누군가 들어 주었을 때처럼");
            AddDefaultExample("기쁨", "고요한", "새벽에 혼자 깨어 있을 때처럼, 잔잔한 호수를 바라볼 때처럼");
            AddDefaultExample("기쁨", "기쁨에넘치는", "아이가 어려운 입학시험에 합격했을 때처럼, 복권에 당첨되었을 때처럼");
            AddDefaultExample("기쁨", "넉넉한", "주머니에 용돈이 투둑할 때처럼");
            AddDefaultExample("기쁨", "다행스런", "못 찾던 중요한 서류를 찾았을 때처럼, 쫓기는 꿈을 꾸다가 깨어났을 때처럼, 걱정했던 병이 정상이라고 판명되었을 때처럼");
            AddDefaultExample("기쁨", "단란한", "가족이 가까운 산으로 피크닉을 갈 때처럼");
            AddDefaultExample("기쁨", "달콤한", "꿀처럼 연인과 함께 마시던 차의 향기처럼, 첫 입맞춤처럼");
            AddDefaultExample("기쁨", "반가운", "오래 못 본 친구를 우연히 길에서 만났을 때처럼, 기다리던 사람이왔을 때처럼, 긴 가뭄 끝에 비가 올 때처럼");
            AddDefaultExample("기쁨", "밝아진", "걱정스러운 일이 잘 해결되고 난 후의 표정처럼");
            AddDefaultExample("기쁨", "상쾌한", "운동 후 샤워를 했을 때처럼, 새벽 공기를 마시며 산책할 때처럼");
            AddDefaultExample("기쁨", "생기도는", "운동을 하고 나서 활력이 넘치는 표정처럼, 아침 이슬 맺힌 나팔꽃처럼");
            AddDefaultExample("기쁨", "시원한", "앓던 이를 뺐을 때처럼, 땀 흘리고 나서 맥주 한잔 할 때처럼");
            AddDefaultExample("기쁨", "신선한", "방금 잡아 올린 물고기처럼, 솔밭에서 솔 향기를 맡을 때처럼, 갓 따온 과일처럼");
            AddDefaultExample("기쁨", "자신만만한", "잘 아는 문제를 풀 때처럼, 나보다 약한 사람과 힘 겨루기를 할 때처럼");
            AddDefaultExample("기쁨", "짜릿짜릿한", "서커스의 묘기를 볼 때처럼, 놀이 기구를 탈 때처럼");
            AddDefaultExample("기쁨", "충족한", "밥을 배불리 먹고 날 때처럼, 적금의 마지막 회를 부을 때처럼, 자동차에 기름을 가득 채웠을 때처럼");
            AddDefaultExample("기쁨", "쾌적한", "깨끗한 새 이불을 덮을 때처럼, 집을 말끔히 치우고 휴식을 취할 때처럼, 경치 좋은 창가에 앉아 차를 마실 때처럼");
            AddDefaultExample("기쁨", "평화로운", "잔잔한 바다를 내다볼 때처럼, 가족과 함께 기도한 후처럼");
            AddDefaultExample("기쁨", "포근한", "흰눈이 소복이 내린 것을 보았을 때처럼, 포옹하고 있을 때처럼, 봄날 양지에 앉아 햇빛을 받을 때처럼");
            AddDefaultExample("기쁨", "풍요로운", "누런 가을 들녘을 볼 때처럼, 온 가족이 다 모였을 때처럼");
            AddDefaultExample("기쁨", "푸짐한", "돼지를 잡아 잔치를 벌렸을 때처럼, 맛있는 음식이 쌓인 것을 볼 때처럼");
            AddDefaultExample("기쁨", "환한", "답답한 터널을 지났을 때처럼, 대낮에 영화관에서 나왔을 때처럼, 해맑은 웃음을 볼 때처럼");

            // 두려움 카테고리
            AddDefaultExample("두려움", "간담이 서늘해지는", "캄캄한 골목길에서 갑자기 사람과 맞닥뜨렸을 때처럼, 부엌칼을 사용하다가 실수로 떨어뜨렸을 때처럼");
            AddDefaultExample("두려움", "걱정스러운", "아이가 12시가 넘도록 전화도 없이 안 들어 올 때처럼");
            AddDefaultExample("두려움", "겁먹은", "장난치다 유리창을 깬 아이가 꾸중을 기다릴 때처럼, 험악하게 생긴 사람이 할 말이 있는 듯 다가올 때처럼");
            AddDefaultExample("두려움", "고생스러운", "홀어머니가 아이들 학교 보내려고 행상을 할 때처럼, 무거운 짐을 지고 험한 길을 걸어갈 때처럼");
            AddDefaultExample("두려움", "근심스러운", "공공요금이 오를 것이라는 뉴스를 들었을 때처럼, 가족 병문안을 갈 때처럼, 의사의 진찰 결과를 기다릴 때처럼");
            AddDefaultExample("두려움", "긴박한", "간첩을 수색하는 군인들의 표정처럼, 추리 소설을 볼 때처럼");
            AddDefaultExample("두려움", "긴장된", "미끄러운 눈길을 걸을 때처럼, 처음 운전대를 잡고 운전할 때처럼");
            AddDefaultExample("두려움", "난감한", "차비가 없을 때처럼, 자동차 면허시험에 여러 번 떨어졌을 때처럼");
            AddDefaultExample("두려움", "냉랭한", "불 때지 않은 방의 윗목처럼, 부부싸움을 하고 돌아 누웠을 때처럼");
            AddDefaultExample("두려움", "답답한", "창없는 방에 오래 앉아 있을 때처럼, 내 말을 믿어주지 않는 사람을 대할 때처럼");
            AddDefaultExample("두려움", "당황한", "우산없이 나갔다가 비를 만났을 때처럼, 몰래 외출하려다가 엄마에게 들켰을 때처럼");
            AddDefaultExample("두려움", "두려운", "성난 파도를 볼 때처럼, 밤길을 혼자 걸을 때처럼, 최후의 심판을 생각할 때처럼");
            AddDefaultExample("두려움", "무안한", "바지 지퍼가 내려져 있음을 알았을 때처럼, 본인이 없는 줄 모르고 헐뜯었는데 바로 옆에 있는 것을 알았을 때처럼");
            AddDefaultExample("두려움", "미안한", "약속 시간에 늦었을 때처럼, 남의 집 일을 도와주다가 그릇을 깼을 때처럼");
            AddDefaultExample("두려움", "불안한", "건강진단을 받고 결과를 보러 갈 때처럼, 늦게까지 오지 않는 남편을 기다릴 때처럼");
            AddDefaultExample("두려움", "서먹서먹한", "처음 출근하여 아는 사람이 하나도 없을 때처럼, 싸웠던 친구와 우연히 같은 차를 타게 되었을 때처럼");
            AddDefaultExample("두려움", "초초한", "길이 막혀 약속 시간에 늦었을 때처럼");

            // 분노 카테고리
            AddDefaultExample("분노", "괘씸한", "달리는 차에 흙탕물이 튀었을 때처럼, 비꼬임을 당할 때처럼");
            AddDefaultExample("분노", "격분한", "억울한 누명을 썼을 때처럼, 면전에서 욕을 먹을 때처럼");
            AddDefaultExample("분노", "골치아픈", "몇 가지 중요한 약속이 중복되었을 때처럼, 일이 자꾸 꼬이기만 할 때처럼");
            AddDefaultExample("분노", "김빠진", "따 놓은지 오래된 맥주처럼, 밤새워 한 숙제를 검사도 하지 않았을 때처럼");
            AddDefaultExample("분노", "답답한", "전화가 잘 들리지 않을 때처럼, 말귀를 못 알아들을 때처럼, 시험 때 TV앞에 앉아 있는 아이를 볼 때처럼");
            AddDefaultExample("분노", "맥빠지는", "배우자가 외식 약속을 취소할 때처럼, 공항에 친구 배웅하러 갔는데 친구는 벌써 떠나고 없을 때처럼");
            AddDefaultExample("분노", "불쾌한", "승낙도 없이 합승하려는 기사를 볼 때처럼");
            AddDefaultExample("분노", "신경질나는", "자꾸 잔소리를 할 때처럼, 새 옷에 김치 국물을 흘렸을 때처럼");
            AddDefaultExample("분노", "짜증스러운", "통근버스를 놓치고 만원버스를 타고 갈 때처럼, 손님 맞을 시간이 되었는데 여기저기 어지러운 집안을 볼 때처럼, 숙제를 미룬 채 놀기에만 열중하는 아이들을 볼 때처럼");
            AddDefaultExample("분노", "화나는", "값비싼 유리컵을 부주의로 깼을 때처럼, 재미있는 TV프로를 보는데 채널을 돌릴 때처럼");
            AddDefaultExample("분노", "황당한", "며칠 찾던 물건이 쓰레기통에 버려져 있을 때처럼, 선물을 바꾸어 전달했을 때처럼, 화장실에서 휴지가 없을 때처럼");

            // 슬픔 카테고리
            AddDefaultExample("슬픔", "비참한", "모든 사람 앞에서 잘못을 인정해야만 할 때처럼, 친구에게 배신 당했을 때처럼");
            AddDefaultExample("슬픔", "서글픈", "낙엽이 뒹구는 길을 홀로 걸을 때처럼, 아이들에게 구세대 취급을 받는구나 하는 생각이 들 때처럼");
            AddDefaultExample("슬픔", "썰렁한", "퇴근 후 아무도 없는 집에 들어설 때처럼, 웃기는 이야기를 했는데 아무도 안 웃을 때처럼");
            AddDefaultExample("슬픔", "처량한", "도살장에 끌려가는 소를 볼 때처럼, 비 오는 날 처마 밑에 앉아 있는 참새를 볼 때처럼");
            AddDefaultExample("슬픔", "허탈한", "정성 들여 준비 했는데 반응이 좋지 않을 때처럼, 회사에 몸바쳐 일했으나 승진에서 탈락된 것을 알았을 때처럼");
            AddDefaultExample("슬픔", "후회스러운", "과음한 다음 날 아침처럼, 부부 싸움을 심하게 하고 난 후처럼");
            AddDefaultExample("슬픔", "쓸쓸한", "추수가 끝난 들판에 홀로 서 있는 허수아비처럼");
            AddDefaultExample("슬픔", "외로운", "길가에 홀로 서 있는 장승처럼");
            AddDefaultExample("슬픔", "고독한", "무인도에 내팽개쳐 있을 때처럼, 춥고 눈오는 날 아무도 없는 들판에 홀로 서 있을 때처럼");
        }

        private void AddDefaultExample(string category, string subCategory, string description)
        {
            _examples.Add(new FeelingExample
            {
                Id = _nextId++,
                Category = category,
                SubCategory = subCategory,
                Description = description,
                IsDefault = true,
                CreatedAt = DateTime.Now
            });
        }

        private async Task SaveDataToFile()
        {
            await _fileStorageService.SaveFeelingExamplesAsync(_examples);
        }

        public async Task<List<FeelingExample>> GetAllExamplesAsync()
        {
            await EnsureInitializedAsync();
            return _examples.ToList();
        }

        public async Task<List<FeelingExample>> GetExamplesByCategoryAsync(string category)
        {
            await EnsureInitializedAsync();
            return _examples.Where(e => e.Category == category).ToList();
        }

        public async Task<FeelingExample> AddExampleAsync(FeelingExample example)
        {
            await EnsureInitializedAsync();
            
            example.Id = _nextId++;
            example.IsDefault = false;
            example.CreatedAt = DateTime.Now;
            
            _examples.Add(example);
            await SaveDataToFile();
            
            return example;
        }

        public async Task<bool> DeleteExampleAsync(int id)
        {
            await EnsureInitializedAsync();
            
            var example = _examples.FirstOrDefault(e => e.Id == id);
            if (example == null || example.IsDefault)
                return false;
            
            _examples.Remove(example);
            await SaveDataToFile();
            
            return true;
        }
    }
}

