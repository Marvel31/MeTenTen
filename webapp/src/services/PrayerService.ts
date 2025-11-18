/**
 * 기도문 관리 서비스 (로컬 스토리지 기반)
 */

import type { Prayer, PrayerCategory } from '../types/prayer';

// 로컬 스토리지 키
const STORAGE_KEY = 'metenten_prayers';

// 기본 기도문 데이터
const DEFAULT_PRAYERS: Omit<Prayer, 'id' | 'createdAt' | 'updatedAt'>[] = [
  // 부부 기도문
  {
    title: '부부의 기도',
    content: `○ 인자하신 하느님 아버지,
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

◎ 아멘.`,
    category: '부부',
    tags: ['부부', '혼인성사', '서약', '사랑'],
    isFavorite: true,
    viewCount: 0,
  },

  // 가정 기도문
  {
    title: '가정을 위한 기도',
    content: `○ 마리아와 요셉에게 순종하시며
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

◎ 아멘.`,
    category: '가정',
    tags: ['가정', '성가정', '성모마리아', '성요셉'],
    isFavorite: true,
    viewCount: 0,
  },

  {
    title: '자녀를 위한 기도',
    content: `○ 세상을 창조하신 하느님,
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

◎ 아멘.`,
    category: '가정',
    tags: ['자녀', '보호', '은총', '양육'],
    isFavorite: false,
    viewCount: 0,
  },

  {
    title: '부모를 위한 기도',
    content: `○ 인자하신 하느님,
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

◎ 아멘.`,
    category: '가정',
    tags: ['부모', '효도', '공경', '은덕'],
    isFavorite: false,
    viewCount: 0,
  },

  // 교회 기도문
  {
    title: '사제들을 위한 기도',
    content: `○ 영원한 사제이신 예수님,
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

◎ 아멘.`,
    category: '교회',
    tags: ['사제', '성직자', '거룩함', '봉헌'],
    isFavorite: false,
    viewCount: 0,
  },

  {
    title: '수도자들을 위한 기도',
    content: `○ 세례성사의 은총을 더욱 풍부하게 열매 맺도록
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

◎ 아멘.`,
    category: '교회',
    tags: ['수도자', '봉헌', '정결', '청빈', '순명'],
    isFavorite: false,
    viewCount: 0,
  },
];

class PrayerService {
  /**
   * 로컬 스토리지에서 기도문 목록 가져오기
   */
  private getFromStorage(): Prayer[] {
    try {
      const data = localStorage.getItem(STORAGE_KEY);
      if (!data) {
        // 초기 데이터 설정
        this.initializeDefaultPrayers();
        return this.getFromStorage();
      }
      return JSON.parse(data);
    } catch (error) {
      console.error('Failed to load prayers from storage:', error);
      return [];
    }
  }

  /**
   * 로컬 스토리지에 기도문 목록 저장
   */
  private saveToStorage(prayers: Prayer[]): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(prayers));
    } catch (error) {
      console.error('Failed to save prayers to storage:', error);
      throw new Error('기도문 저장에 실패했습니다.');
    }
  }

  /**
   * 기본 기도문 초기화
   */
  private initializeDefaultPrayers(): void {
    const now = new Date().toISOString();
    const prayers: Prayer[] = DEFAULT_PRAYERS.map((prayer, index) => ({
      ...prayer,
      id: `prayer_${index}`,
      createdAt: now,
      updatedAt: now,
    }));
    this.saveToStorage(prayers);
  }

  /**
   * 모든 기도문 조회
   */
  async getAllPrayers(): Promise<Prayer[]> {
    return this.getFromStorage();
  }

  /**
   * ID로 기도문 조회
   */
  async getPrayerById(id: string): Promise<Prayer | null> {
    const prayers = this.getFromStorage();
    return prayers.find((p) => p.id === id) || null;
  }

  /**
   * 카테고리별 기도문 조회
   */
  async getPrayersByCategory(category: string): Promise<Prayer[]> {
    const prayers = this.getFromStorage();
    return prayers.filter((p) => p.category === category);
  }

  /**
   * 카테고리 목록 조회
   */
  async getCategories(): Promise<PrayerCategory[]> {
    const prayers = this.getFromStorage();
    const categoryMap = new Map<string, Prayer[]>();

    prayers.forEach((prayer) => {
      if (!categoryMap.has(prayer.category)) {
        categoryMap.set(prayer.category, []);
      }
      categoryMap.get(prayer.category)!.push(prayer);
    });

    const categories: PrayerCategory[] = [];
    categoryMap.forEach((prayers, categoryName) => {
      categories.push({
        id: categoryName,
        name: categoryName,
        prayers: prayers.sort((a, b) => a.title.localeCompare(b.title)),
      });
    });

    // 카테고리 순서 정렬
    const order = ['부부', '가정', '교회'];
    categories.sort((a, b) => {
      const indexA = order.indexOf(a.name);
      const indexB = order.indexOf(b.name);
      if (indexA === -1) return 1;
      if (indexB === -1) return -1;
      return indexA - indexB;
    });

    return categories;
  }

  /**
   * 조회수 증가
   */
  async incrementViewCount(id: string): Promise<void> {
    const prayers = this.getFromStorage();
    const prayer = prayers.find((p) => p.id === id);
    if (prayer) {
      prayer.viewCount += 1;
      prayer.updatedAt = new Date().toISOString();
      this.saveToStorage(prayers);
    }
  }

  /**
   * 즐겨찾기 토글
   */
  async toggleFavorite(id: string): Promise<void> {
    const prayers = this.getFromStorage();
    const prayer = prayers.find((p) => p.id === id);
    if (prayer) {
      prayer.isFavorite = !prayer.isFavorite;
      prayer.updatedAt = new Date().toISOString();
      this.saveToStorage(prayers);
    }
  }

  /**
   * 즐겨찾기 기도문 조회
   */
  async getFavoritePrayers(): Promise<Prayer[]> {
    const prayers = this.getFromStorage();
    return prayers.filter((p) => p.isFavorite);
  }

  /**
   * 모든 데이터 초기화 (기본값으로 복원)
   */
  async resetToDefault(): Promise<void> {
    localStorage.removeItem(STORAGE_KEY);
    this.initializeDefaultPrayers();
  }
}

export const prayerService = new PrayerService();

