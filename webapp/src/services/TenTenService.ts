/**
 * TenTen 서비스
 * TenTen CRUD 비즈니스 로직 및 암호화 처리
 */

import { firebaseDataService } from './FirebaseDataService';
import { encryptionService } from './EncryptionService';
import { topicService } from './TopicService';
import type {
  TenTen,
  CreateTenTenRequest,
  UpdateTenTenRequest,
  DecryptedTenTen,
  EncryptionType,
} from '../types';

class TenTenService {
  /**
   * 사용자의 모든 TenTen 조회 (복호화 포함)
   */
  async getTenTens(userId: string): Promise<DecryptedTenTen[]> {
    // 암호화된 TenTen 목록 조회
    const encryptedTenTens = await firebaseDataService.getTenTens(userId);

    // 각 TenTen 복호화
    const decryptedTenTens: DecryptedTenTen[] = await Promise.all(
      encryptedTenTens.map(async (tenten) => {
        const decryptedContent = await this.decryptContent(
          tenten.content,
          tenten.encryptionType
        );

        return {
          ...tenten,
          content: decryptedContent,
        };
      })
    );

    return decryptedTenTens;
  }

  /**
   * 특정 Topic의 모든 TenTen 조회 (복호화 포함)
   */
  async getTenTensByTopic(
    userId: string,
    topicId: string
  ): Promise<DecryptedTenTen[]> {
    // 암호화된 TenTen 목록 조회
    const encryptedTenTens = await firebaseDataService.getTenTensByTopic(
      userId,
      topicId
    );

    // 각 TenTen 복호화
    const decryptedTenTens: DecryptedTenTen[] = await Promise.all(
      encryptedTenTens.map(async (tenten) => {
        const decryptedContent = await this.decryptContent(
          tenten.content,
          tenten.encryptionType
        );

        return {
          ...tenten,
          content: decryptedContent,
        };
      })
    );

    return decryptedTenTens;
  }

  /**
   * 특정 TenTen 조회 (복호화 포함)
   */
  async getTenTenById(
    userId: string,
    tenTenId: string
  ): Promise<DecryptedTenTen | null> {
    // 암호화된 TenTen 조회
    const encryptedTenTen = await firebaseDataService.getTenTenById(
      userId,
      tenTenId
    );

    if (!encryptedTenTen) {
      return null;
    }

    // 복호화
    const decryptedContent = await this.decryptContent(
      encryptedTenTen.content,
      encryptedTenTen.encryptionType
    );

    return {
      ...encryptedTenTen,
      content: decryptedContent,
    };
  }

  /**
   * TenTen 생성 (암호화 포함)
   */
  async createTenTen(
    userId: string,
    userName: string,
    request: CreateTenTenRequest
  ): Promise<TenTen> {
    // 유효성 검증
    if (!request.content || request.content.trim().length === 0) {
      throw new Error('내용을 입력해주세요.');
    }

    if (!request.topicId) {
      throw new Error('주제를 선택해주세요.');
    }

    if (!request.encryptionType) {
      throw new Error('암호화 타입을 선택해주세요.');
    }

    // Topic 조회 (topicSubject 확인)
    const topic = await topicService.getTopicById(userId, request.topicId);
    if (!topic) {
      throw new Error('주제를 찾을 수 없습니다.');
    }

    // 암호화 타입별 처리
    let encryptedContent: string;
    if (request.encryptionType === 'shared') {
      // Shared DEK로 암호화
      if (!encryptionService.hasSharedDEK()) {
        throw new Error('배우자가 연결되지 않았습니다.');
      }
      encryptedContent = await encryptionService.encryptWithSharedDEK(
        request.content.trim()
      );
    } else {
      // Personal DEK로 암호화
      if (!encryptionService.hasDEK()) {
        throw new Error('DEK가 설정되지 않았습니다.');
      }
      encryptedContent = await encryptionService.encrypt(
        request.content.trim()
      );
    }

    // TenTen 생성 요청 데이터 준비
    const createRequest: CreateTenTenRequest = {
      content: encryptedContent, // 암호화된 내용 (Base64)
      topicId: request.topicId,
      encryptionType: request.encryptionType,
    };

    // Firebase에 저장
    const firebaseKey = await firebaseDataService.createTenTen(
      userId,
      userName,
      topic.subject,
      createRequest
    );

    // 생성된 TenTen 조회
    const tenten = await firebaseDataService.getTenTenById(userId, firebaseKey);
    if (!tenten) {
      throw new Error('TenTen 생성 후 조회에 실패했습니다.');
    }

    return tenten;
  }

  /**
   * TenTen 수정 (암호화 포함)
   */
  async updateTenTen(
    userId: string,
    tenTenId: string,
    request: UpdateTenTenRequest
  ): Promise<TenTen> {
    // 기존 TenTen 조회
    const existingTenTen = await firebaseDataService.getTenTenById(
      userId,
      tenTenId
    );
    if (!existingTenTen) {
      throw new Error('TenTen을 찾을 수 없습니다.');
    }

    // 유효성 검증
    if (!request.content || request.content.trim().length === 0) {
      throw new Error('내용을 입력해주세요.');
    }

    // 기존 암호화 타입 사용
    const encryptionType = existingTenTen.encryptionType;

    // 암호화 타입별 처리
    let encryptedContent: string;
    if (encryptionType === 'shared') {
      // Shared DEK로 암호화
      if (!encryptionService.hasSharedDEK()) {
        throw new Error('배우자가 연결되지 않았습니다.');
      }
      encryptedContent = await encryptionService.encryptWithSharedDEK(
        request.content.trim()
      );
    } else {
      // Personal DEK로 암호화
      if (!encryptionService.hasDEK()) {
        throw new Error('DEK가 설정되지 않았습니다.');
      }
      encryptedContent = await encryptionService.encrypt(
        request.content.trim()
      );
    }

    // TenTen 수정 요청 데이터 준비
    const updateRequest: UpdateTenTenRequest = {
      content: encryptedContent, // 암호화된 내용 (Base64)
    };

    // Firebase에 저장
    await firebaseDataService.updateTenTen(userId, tenTenId, updateRequest);

    // 수정된 TenTen 조회
    const updatedTenTen = await firebaseDataService.getTenTenById(
      userId,
      tenTenId
    );
    if (!updatedTenTen) {
      throw new Error('TenTen 수정 후 조회에 실패했습니다.');
    }

    return updatedTenTen;
  }

  /**
   * TenTen 삭제
   */
  async deleteTenTen(userId: string, tenTenId: string): Promise<void> {
    // 기존 TenTen 조회
    const existingTenTen = await firebaseDataService.getTenTenById(
      userId,
      tenTenId
    );
    if (!existingTenTen) {
      throw new Error('TenTen을 찾을 수 없습니다.');
    }

    // TenTen 삭제
    await firebaseDataService.deleteTenTen(userId, tenTenId);
  }

  /**
   * TenTen 내용 복호화 (암호화 타입별)
   */
  private async decryptContent(
    encryptedContent: string,
    encryptionType: EncryptionType
  ): Promise<string> {
    if (encryptionType === 'shared') {
      // Shared DEK로 복호화
      if (!encryptionService.hasSharedDEK()) {
        throw new Error('배우자가 연결되지 않았습니다.');
      }
      return await encryptionService.decryptWithSharedDEK(encryptedContent);
    } else {
      // Personal DEK로 복호화
      if (!encryptionService.hasDEK()) {
        throw new Error('DEK가 설정되지 않았습니다.');
      }
      return await encryptionService.decrypt(encryptedContent);
    }
  }

  /**
   * TenTen 읽음 표시 업데이트
   */
  async markTenTenAsRead(
    userId: string,
    tenTenId: string
  ): Promise<void> {
    await firebaseDataService.markTenTenAsRead(userId, tenTenId);
  }

  /**
   * Topic에 TenTen이 있는지 확인
   */
  async hasTenTenForTopic(
    userId: string,
    topicId: string
  ): Promise<boolean> {
    const tentens = await firebaseDataService.getTenTensByTopic(
      userId,
      topicId
    );
    return tentens.length > 0;
  }

  /**
   * 사용자의 전체 TenTen 개수 조회
   */
  async getTenTenCount(userId: string): Promise<number> {
    const tentens = await firebaseDataService.getTenTens(userId);
    return tentens.length;
  }

  /**
   * 특정 Topic의 TenTen 개수 조회
   */
  async getTenTenCountByTopic(
    userId: string,
    topicId: string
  ): Promise<number> {
    const tentens = await firebaseDataService.getTenTensByTopic(
      userId,
      topicId
    );
    return tentens.length;
  }
}

// 싱글톤 인스턴스
export const tenTenService = new TenTenService();
export default tenTenService;

