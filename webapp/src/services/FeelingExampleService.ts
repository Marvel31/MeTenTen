/**
 * 느낌 표현 예시 관리 서비스 (로컬 스토리지 기반)
 */

import type {
  FeelingExample,
  FeelingCategory,
  CreateFeelingExampleRequest,
  FeelingCategoryInfo,
} from '../types/feeling';

// 로컬 스토리지 키
const STORAGE_KEY = 'metenten_feeling_examples';

// 기본 감정 예시 데이터 (초기 데이터)
const DEFAULT_EXAMPLES: Omit<FeelingExample, 'id' | 'createdAt' | 'updatedAt'>[] = [
  // 기쁨 (Joy)
  { category: 'joy', subCategory: '가벼운', description: '새털처럼 빈 기방을 들 때처럼, 힘든 숙제를 다 마쳤을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '개운한', description: '목욕한 다음처럼, 고해성사를 보았을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '감격스런', description: '선생님께 뜻하지 않게 칭찬을 들었을 때처럼, 올림픽 시상식에서 태극기가 올라가고 애국가가 울려 펴질 때처럼', isDefault: true },
  { category: 'joy', subCategory: '감동을받은', description: '원하던 선물을 배우자에게 받았을 때처럼, 뜻밖에도 말썽꾸러기 아들로부터 사랑한다는 편지를 받았을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '경이로운', description: '바다 속 신비의 세계를 보았을 때처럼, 첫 출산 후 아이를 볼 때처럼', isDefault: true },
  { category: 'joy', subCategory: '경쾌한', description: '아침에 일어나 행진곡을 들을 때처럼, 리듬 체조를 하는 여자 선수의 동작을 볼 때처럼', isDefault: true },
  { category: 'joy', subCategory: '고마운', description: '길을 친절하게 안내 받았을 때처럼, 무거운 짐을 누군가 들어 주었을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '고요한', description: '새벽에 혼자 깨어 있을 때처럼, 잔잔한 호수를 바라볼 때처럼', isDefault: true },
  { category: 'joy', subCategory: '기쁨에넘치는', description: '아이가 어려운 입학시험에 합격했을 때처럼, 복권에 당첨되었을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '넉넉한', description: '주머니에 용돈이 투둑하게 있을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '다행스런', description: '못 찾던 중요한 서류를 찾았을 때처럼, 쫓기는 꿈을 꾸다가 깨어났을 때처럼, 걱정했던 병이 정상이라고 판명되었을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '단란한', description: '가족이 가까운 산으로 피크닉을 갈 때처럼', isDefault: true },
  { category: 'joy', subCategory: '달콤한', description: '꿀처럼 연인과 함께 마시던 차의 향기처럼, 첫 입맞춤처럼', isDefault: true },
  { category: 'joy', subCategory: '반가운', description: '오래 못 본 친구를 우연히 길에서 만났을 때처럼, 기다리던 사람이 왔을 때처럼, 긴 가뭄 끝에 비가 올 때처럼', isDefault: true },
  { category: 'joy', subCategory: '밝아진', description: '걱정스러운 일이 잘 해결되고 난 후의 표정처럼', isDefault: true },
  { category: 'joy', subCategory: '상쾌한', description: '운동 후 샤워를 했을 때처럼, 새벽 공기를 마시며 산책할 때처럼', isDefault: true },
  { category: 'joy', subCategory: '생기도는', description: '운동을 하고 나서 활력이 넘치는 표정처럼, 아침 이슬 맺힌 나팔꽃처럼', isDefault: true },
  { category: 'joy', subCategory: '시원한', description: '앓던 이를 뺐을 때처럼, 땀 흘리고 나서 맥주 한잔 할 때처럼', isDefault: true },
  { category: 'joy', subCategory: '신선한', description: '방금 잡아 올린 물고기처럼, 솔밭에서 솔 향기를 맡을 때처럼, 갓 따온 과일처럼', isDefault: true },
  { category: 'joy', subCategory: '자신만만한', description: '잘 아는 문제를 풀 때처럼, 나보다 약한 사람과 힘 겨루기를 할 때처럼', isDefault: true },
  { category: 'joy', subCategory: '짜릿짜릿한', description: '서커스의 묘기를 볼 때처럼, 놀이 기구를 탈 때처럼', isDefault: true },
  { category: 'joy', subCategory: '충족한', description: '밥을 배불리 먹고 날 때처럼, 적금의 마지막 회를 부을 때처럼, 자동차에 기름을 가득 채웠을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '쾌적한', description: '깨끗한 새 이불을 덮을 때처럼, 집을 말끔히 치우고 휴식을 취할 때처럼, 경치 좋은 창가에 앉아 차를 마실 때처럼', isDefault: true },
  { category: 'joy', subCategory: '평화로운', description: '잔잔한 바다를 내다볼 때처럼, 가족과 함께 기도한 후처럼', isDefault: true },
  { category: 'joy', subCategory: '포근한', description: '흰눈이 소복이 내린 것을 보았을 때처럼, 포옹하고 있을 때처럼, 봄날 양지에 앉아 햇볕을 받을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '풍요로운', description: '누런 가을 들녘을 볼 때처럼, 온 가족이 다 모였을 때처럼', isDefault: true },
  { category: 'joy', subCategory: '푸짐한', description: '돼지를 잡아 잔치를 벌렸을 때처럼, 맛있는 음식이 쌓인 것을 볼 때처럼', isDefault: true },
  { category: 'joy', subCategory: '환한', description: '답답한 터널을 지났을 때처럼, 대낮에 영화관에서 나왔을 때처럼, 해맑은 웃음을 볼 때처럼', isDefault: true },

  // 두려움 (Fear)
  { category: 'fear', subCategory: '간담이 서늘해지는', description: '캄캄한 골목길에서 갑자기 사람과 맞닥뜨렸을 때처럼, 부엌칼을 사용하다가 실수로 떨어뜨렸을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '걱정스러운', description: '아이가 12시가 넘도록 전화도 없이 안 들어올 때처럼', isDefault: true },
  { category: 'fear', subCategory: '겁먹은', description: '장난치다 유리창을 깬 아이가 꾸중을 기다릴 때처럼, 험악하게 생긴 사람이 할 말이 있는 듯 다가올 때처럼', isDefault: true },
  { category: 'fear', subCategory: '고생스러운', description: '홀어머니가 아이들 학교 보내려고 행상을 할 때처럼, 무거운 짐을 지고 험한 길을 걸어갈 때처럼', isDefault: true },
  { category: 'fear', subCategory: '근심스러운', description: '공공요금이 오를 것이라는 뉴스를 들었을 때처럼, 가족 병문안을 갈 때처럼, 의사의 진찰 결과를 기다릴 때처럼', isDefault: true },
  { category: 'fear', subCategory: '긴박한', description: '간첩을 수색하는 군인들의 표정처럼, 추리 소설을 볼 때처럼', isDefault: true },
  { category: 'fear', subCategory: '긴장된', description: '미끄러운 눈길을 걸을 때처럼, 처음 운전대를 잡고 운전할 때처럼', isDefault: true },
  { category: 'fear', subCategory: '난감한', description: '차비가 없을 때처럼, 자동차 면허시험에 여러 번 떨어졌을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '냉랭한', description: '불 때지 않은 방의 윗목처럼, 부부싸움을 하고 돌아 누웠을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '답답한', description: '창없는 방에 오래 앉아 있을 때처럼, 내 말을 믿어주지 않는 사람을 대할 때처럼', isDefault: true },
  { category: 'fear', subCategory: '당황한', description: '우산없이 나갔다가 비를 만났을 때처럼, 몰래 외출하려다가 엄마에게 들켰을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '두려운', description: '성난 파도를 볼 때처럼, 밤길을 혼자 걸을 때처럼, 최후의 심판을 생각할 때처럼', isDefault: true },
  { category: 'fear', subCategory: '무안한', description: '바지 지퍼가 내려져 있음을 알았을 때처럼, 본인이 없는 줄 모르고 헐뜯었는데 바로 옆에 있는 것을 알았을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '미안한', description: '약속 시간에 늦었을 때처럼, 남의 집 일을 도와주다가 그릇을 깼을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '불안한', description: '건강진단을 받고 결과를 보러 갈 때처럼, 늦게까지 오지 않는 남편을 기다릴 때처럼', isDefault: true },
  { category: 'fear', subCategory: '서먹서먹한', description: '처음 출근하여 아는 사람이 하나도 없을 때처럼, 싸웠던 친구와 우연히 같은 차를 타게 되었을 때처럼', isDefault: true },
  { category: 'fear', subCategory: '초초한', description: '길이 막혀 약속 시간에 늦었을 때처럼', isDefault: true },

  // 분노 (Anger)
  { category: 'anger', subCategory: '괘씸한', description: '달리는 차에 흙탕물이 튀었을 때처럼, 비꼬임을 당할 때처럼', isDefault: true },
  { category: 'anger', subCategory: '격분한', description: '억울한 누명을 썼을 때처럼, 면전에서 욕을 먹을 때처럼', isDefault: true },
  { category: 'anger', subCategory: '골치아픈', description: '몇 가지 중요한 약속이 중복되었을 때처럼, 일이 자꾸 꼬이기만 할 때처럼', isDefault: true },
  { category: 'anger', subCategory: '김빠진', description: '따 놓은지 오래된 맥주처럼, 밤새워 한 숙제를 검사도 하지 않았을 때처럼', isDefault: true },
  { category: 'anger', subCategory: '답답한', description: '전화가 잘 들리지 않을 때처럼, 말귀를 못 알아들을 때처럼, 시험 때 TV앞에 앉아 있는 아이를 볼 때처럼', isDefault: true },
  { category: 'anger', subCategory: '맥빠지는', description: '배우자가 외식 약속을 취소할 때처럼, 공항에 친구 배웅하러 갔는데 친구는 벌써 떠나고 없을 때처럼', isDefault: true },
  { category: 'anger', subCategory: '불쾌한', description: '승낙도 없이 합승하려는 기사를 볼 때처럼', isDefault: true },
  { category: 'anger', subCategory: '신경질나는', description: '자꾸 잔소리를 할 때처럼, 새 옷에 김치 국물을 흘렸을 때처럼', isDefault: true },
  { category: 'anger', subCategory: '짜증스러운', description: '통근버스를 놓치고 만원버스를 타고 갈 때처럼, 손님 맞을 시간이 되었는데 여기저기 어지러운 집안을 볼 때처럼, 숙제를 미룬 채 놀기에만 열중하는 아이들을 볼 때처럼', isDefault: true },
  { category: 'anger', subCategory: '화나는', description: '값비싼 유리컵을 부주의로 깼을 때처럼, 재미있는 TV프로를 보는데 채널을 돌릴 때처럼', isDefault: true },
  { category: 'anger', subCategory: '황당한', description: '며칠 찾던 물건이 쓰레기통에 버려져 있을 때처럼, 선물을 바꾸어 전달했을 때처럼, 화장실에서 휴지가 없을 때처럼', isDefault: true },

  // 슬픔 (Sadness)
  { category: 'sadness', subCategory: '비참한', description: '모든 사람 앞에서 잘못을 인정해야만 할 때처럼, 친구에게 배신 당했을 때처럼', isDefault: true },
  { category: 'sadness', subCategory: '서글픈', description: '낙엽이 뒹구는 길을 홀로 걸을 때처럼, 아이들에게 구세대 취급을 받는구나 하는 생각이 들 때처럼', isDefault: true },
  { category: 'sadness', subCategory: '썰렁한', description: '퇴근 후 아무도 없는 집에 들어설 때처럼, 웃기는 이야기를 했는데 아무도 안 웃을 때처럼', isDefault: true },
  { category: 'sadness', subCategory: '처량한', description: '도살장에 끌려가는 소를 볼 때처럼, 비 오는 날 처마 밑에 앉아 있는 참새를 볼 때처럼', isDefault: true },
  { category: 'sadness', subCategory: '허탈한', description: '정성 들여 준비 했는데 반응이 좋지 않을 때처럼, 회사에 몸바쳐 일했으나 승진에서 탈락된 것을 알았을 때처럼', isDefault: true },
  { category: 'sadness', subCategory: '후회스러운', description: '과음한 다음 날 아침처럼, 부부 싸움을 심하게 하고 난 후처럼', isDefault: true },
  { category: 'sadness', subCategory: '쓸쓸한', description: '추수가 끝난 들판에 홀로 서 있는 허수아비처럼', isDefault: true },
  { category: 'sadness', subCategory: '외로운', description: '길가에 홀로 서 있는 장승처럼', isDefault: true },
  { category: 'sadness', subCategory: '고독한', description: '무인도에 내팽개쳐져 있을 때처럼, 춥고 눈오는 날 아무도 없는 들판에 홀로 서 있을 때처럼', isDefault: true },
];

// 카테고리 정보
const CATEGORY_INFO: Record<FeelingCategory, { displayName: string; emoji: string }> = {
  joy: { displayName: '기쁨', emoji: '😊' },
  fear: { displayName: '두려움', emoji: '😰' },
  anger: { displayName: '분노', emoji: '😠' },
  sadness: { displayName: '슬픔', emoji: '😢' },
};

class FeelingExampleService {
  /**
   * 로컬 스토리지에서 감정 예시 목록 가져오기
   */
  private getFromStorage(): FeelingExample[] {
    try {
      const data = localStorage.getItem(STORAGE_KEY);
      if (!data) {
        // 초기 데이터 설정
        this.initializeDefaultExamples();
        return this.getFromStorage();
      }
      return JSON.parse(data);
    } catch (error) {
      console.error('Failed to load feeling examples from storage:', error);
      return [];
    }
  }

  /**
   * 로컬 스토리지에 감정 예시 목록 저장
   */
  private saveToStorage(examples: FeelingExample[]): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(examples));
    } catch (error) {
      console.error('Failed to save feeling examples to storage:', error);
      throw new Error('감정 예시 저장에 실패했습니다.');
    }
  }

  /**
   * 기본 감정 예시 초기화
   */
  private initializeDefaultExamples(): void {
    const now = new Date().toISOString();
    const examples: FeelingExample[] = DEFAULT_EXAMPLES.map((ex, index) => ({
      ...ex,
      id: `default_${index}`,
      createdAt: now,
      updatedAt: now,
    }));
    this.saveToStorage(examples);
  }

  /**
   * 모든 감정 예시 조회
   */
  async getAllExamples(): Promise<FeelingExample[]> {
    return this.getFromStorage();
  }

  /**
   * 카테고리별 감정 예시 조회
   */
  async getExamplesByCategory(category: FeelingCategory): Promise<FeelingExample[]> {
    const examples = this.getFromStorage();
    return examples.filter((ex) => ex.category === category);
  }

  /**
   * 카테고리 정보와 함께 감정 예시 조회
   */
  async getCategoryInfoList(): Promise<FeelingCategoryInfo[]> {
    const examples = this.getFromStorage();
    const categories: FeelingCategory[] = ['joy', 'fear', 'anger', 'sadness'];

    return categories.map((category) => ({
      category,
      displayName: CATEGORY_INFO[category].displayName,
      emoji: CATEGORY_INFO[category].emoji,
      examples: examples.filter((ex) => ex.category === category),
    }));
  }

  /**
   * 감정 예시 추가
   */
  async createExample(request: CreateFeelingExampleRequest): Promise<FeelingExample> {
    const examples = this.getFromStorage();
    const now = new Date().toISOString();
    const newId = `custom_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

    const newExample: FeelingExample = {
      id: newId,
      category: request.category,
      subCategory: request.subCategory,
      description: request.description,
      isDefault: false,
      createdAt: now,
      updatedAt: now,
    };

    examples.push(newExample);
    this.saveToStorage(examples);

    return newExample;
  }

  /**
   * 감정 예시 삭제 (기본 예시는 삭제 불가)
   */
  async deleteExample(id: string): Promise<void> {
    const examples = this.getFromStorage();
    const example = examples.find((ex) => ex.id === id);

    if (!example) {
      throw new Error('감정 예시를 찾을 수 없습니다.');
    }

    if (example.isDefault) {
      throw new Error('기본 감정 예시는 삭제할 수 없습니다.');
    }

    const filteredExamples = examples.filter((ex) => ex.id !== id);
    this.saveToStorage(filteredExamples);
  }

  /**
   * 모든 데이터 초기화 (기본값으로 복원)
   */
  async resetToDefault(): Promise<void> {
    localStorage.removeItem(STORAGE_KEY);
    this.initializeDefaultExamples();
  }
}

export const feelingExampleService = new FeelingExampleService();

