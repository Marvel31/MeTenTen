/**
 * Topic 서비스
 * Topic CRUD 비즈니스 로직
 */

import { firebaseDataService } from './FirebaseDataService';
import type {
  Topic,
  CreateTopicRequest,
  UpdateTopicRequest,
  TopicFilter,
} from '../types/topic';
import { parseISO, format, isValid } from 'date-fns';

class TopicService {
  /**
   * Topic 목록 조회 (필터링 및 정렬 지원)
   */
  async getTopics(
    userId: string,
    filter?: TopicFilter,
    sortBy: 'date' | 'createdAt' = 'date',
    sortOrder: 'asc' | 'desc' = 'desc'
  ): Promise<Topic[]> {
    // 1. 모든 Topic 조회
    let topics = await firebaseDataService.getTopics(userId);

    // 2. 필터링
    if (filter) {
      // isActive 필터
      if (filter.isActive !== undefined) {
        topics = topics.filter((topic) => topic.isActive === filter.isActive);
      }

      // 월별 필터
      if (filter.year && filter.month) {
        topics = topics.filter((topic) => {
          const topicDate = parseISO(topic.topicDate);
          if (!isValid(topicDate)) {
            return false;
          }
          return (
            topicDate.getFullYear() === filter.year &&
            topicDate.getMonth() + 1 === filter.month
          );
        });
      }
    }

    // 3. 정렬
    topics.sort((a, b) => {
      let aValue: string;
      let bValue: string;

      if (sortBy === 'date') {
        aValue = a.topicDate;
        bValue = b.topicDate;
      } else {
        // createdAt
        aValue = a.createdAt;
        bValue = b.createdAt;
      }

      if (sortOrder === 'asc') {
        return aValue.localeCompare(bValue);
      } else {
        // desc
        return bValue.localeCompare(aValue);
      }
    });

    // 4. id 부여 (로컬 ID)
    return topics.map((topic, index) => ({
      ...topic,
      id: index + 1, // 임시 ID (실제로는 UUID나 타임스탬프 기반 ID 사용 권장)
    }));
  }

  /**
   * 특정 Topic 조회
   */
  async getTopicById(userId: string, topicId: string): Promise<Topic | null> {
    const topic = await firebaseDataService.getTopicById(userId, topicId);
    if (!topic) {
      return null;
    }

    // id 부여 (로컬 ID)
    return {
      ...topic,
      id: 1, // 임시 ID
    };
  }

  /**
   * Topic 생성
   */
  async createTopic(
    userId: string,
    request: CreateTopicRequest
  ): Promise<Topic> {
    // 유효성 검증
    if (!request.subject || request.subject.trim().length === 0) {
      throw new Error('주제를 입력해주세요.');
    }

    if (!request.topicDate) {
      throw new Error('날짜를 선택해주세요.');
    }

    // 날짜 형식 검증
    const topicDate = parseISO(request.topicDate);
    if (!isValid(topicDate)) {
      throw new Error('유효하지 않은 날짜 형식입니다.');
    }

    // 날짜를 YYYY-MM-DD 형식으로 정규화
    const normalizedDate = format(topicDate, 'yyyy-MM-dd');

    // Topic 생성
    const firebaseKey = await firebaseDataService.createTopic(userId, {
      subject: request.subject.trim(),
      topicDate: normalizedDate,
    });

    // 생성된 Topic 조회
    const topic = await firebaseDataService.getTopicById(userId, firebaseKey);
    if (!topic) {
      throw new Error('Topic 생성 후 조회에 실패했습니다.');
    }

    return {
      ...topic,
      id: 1, // 임시 ID
    };
  }

  /**
   * Topic 수정
   */
  async updateTopic(
    userId: string,
    topicId: string,
    request: UpdateTopicRequest
  ): Promise<Topic> {
    // 기존 Topic 조회
    const existingTopic = await firebaseDataService.getTopicById(
      userId,
      topicId
    );
    if (!existingTopic) {
      throw new Error('Topic을 찾을 수 없습니다.');
    }

    // 수정할 데이터 준비
    const updateData: UpdateTopicRequest = {};

    if (request.subject !== undefined) {
      if (!request.subject || request.subject.trim().length === 0) {
        throw new Error('주제를 입력해주세요.');
      }
      updateData.subject = request.subject.trim();
    }

    if (request.topicDate !== undefined) {
      if (!request.topicDate) {
        throw new Error('날짜를 선택해주세요.');
      }

      // 날짜 형식 검증
      const topicDate = parseISO(request.topicDate);
      if (!isValid(topicDate)) {
        throw new Error('유효하지 않은 날짜 형식입니다.');
      }

      // 날짜를 YYYY-MM-DD 형식으로 정규화
      updateData.topicDate = format(topicDate, 'yyyy-MM-dd');
    }

    // 수정할 데이터가 없으면 기존 Topic 반환
    if (Object.keys(updateData).length === 0) {
      return {
        ...existingTopic,
        id: 1, // 임시 ID
      };
    }

    // Topic 수정
    await firebaseDataService.updateTopic(userId, topicId, updateData);

    // 수정된 Topic 조회
    const updatedTopic = await firebaseDataService.getTopicById(
      userId,
      topicId
    );
    if (!updatedTopic) {
      throw new Error('Topic 수정 후 조회에 실패했습니다.');
    }

    return {
      ...updatedTopic,
      id: 1, // 임시 ID
    };
  }

  /**
   * Topic 삭제 (Hard Delete)
   * 관련된 모든 TenTen도 함께 삭제
   */
  async deleteTopic(userId: string, topicId: string): Promise<void> {
    // 기존 Topic 조회
    const existingTopic = await firebaseDataService.getTopicById(
      userId,
      topicId
    );
    if (!existingTopic) {
      throw new Error('Topic을 찾을 수 없습니다.');
    }

    // 해당 Topic의 모든 TenTen 조회
    const tentens = await firebaseDataService.getTenTensByTopic(
      userId,
      topicId
    );

    // 모든 TenTen 삭제
    const deleteTenTensPromises = tentens.map((tenten) =>
      firebaseDataService.deleteTenTen(userId, tenten.firebaseKey)
    );
    await Promise.all(deleteTenTensPromises);

    // Topic 삭제 - Firebase에서 완전히 삭제
    await firebaseDataService.deleteTopic(userId, topicId);
  }

  /**
   * Topic 완료 상태 확인 (TenTen 존재 여부)
   * 향후 TenTenService와 연동하여 구현
   */
  async checkTopicCompletion(_userId: string, _topicId: string): Promise<boolean> {
    // TODO: TenTenService와 연동하여 해당 Topic에 TenTen이 있는지 확인
    // 현재는 항상 false 반환
    return false;
  }

  /**
   * 현재 월의 Topic 목록 조회
   */
  async getCurrentMonthTopics(userId: string): Promise<Topic[]> {
    const now = new Date();
    return await this.getTopics(userId, {
      year: now.getFullYear(),
      month: now.getMonth() + 1,
      isActive: true,
    });
  }

  /**
   * 월별 Topic 개수 조회
   */
  async getTopicCountByMonth(
    userId: string,
    year: number,
    month: number
  ): Promise<number> {
    const topics = await this.getTopics(
      userId,
      {
        year,
        month,
        isActive: true,
      },
      'date',
      'desc'
    );
    return topics.length;
  }
}

// 싱글톤 인스턴스
export const topicService = new TopicService();
export default topicService;

