/**
 * Firebase Realtime Database 연동 서비스
 */

import { database } from '@config/firebase';
import {
  ref,
  set,
  get,
  update,
  remove,
  query,
  orderByChild,
  equalTo,
  child,
  push,
} from 'firebase/database';
import type { User, PartnerInfo, PendingSharedDEK } from '../types/user';
import type {
  Topic,
  CreateTopicRequest,
  UpdateTopicRequest,
} from '../types/topic';
import type {
  TenTen,
  CreateTenTenRequest,
  UpdateTenTenRequest,
} from '../types/tenten';
import { getCurrentDateTime } from '@utils/date';

class FirebaseDataService {
  // ========== User & DEK 관리 ==========

  /**
   * 사용자 DEK 저장 (회원가입 시)
   */
  async saveUserDEK(
    userId: string,
    email: string,
    displayName: string,
    encryptedDEK: string
  ): Promise<void> {
    const userRef = ref(database, `users/${userId}`);
    const userData = {
      email,
      displayName,
      encryptedDEK,
      createdAt: getCurrentDateTime(),
      updatedAt: getCurrentDateTime(),
    };

    await set(userRef, userData);
  }

  /**
   * 사용자 DEK 조회
   */
  async getUserDEK(userId: string): Promise<User | null> {
    const userRef = ref(database, `users/${userId}`);
    const snapshot = await get(userRef);

    if (snapshot.exists()) {
      const data = snapshot.val();
      return {
        uid: userId,
        ...data,
      } as User;
    }

    return null;
  }

  /**
   * 사용자 DEK 업데이트 (비밀번호 변경 시)
   */
  async updateUserDEK(userId: string, encryptedDEK: string): Promise<void> {
    const userRef = ref(database, `users/${userId}`);
    await update(userRef, {
      encryptedDEK,
      updatedAt: getCurrentDateTime(),
    });
  }

  /**
   * 이메일로 사용자 검색 (이메일, displayName만 반환)
   * 배우자 초대 시 사용 (민감한 정보는 제외)
   */
  async getUserByEmail(email: string): Promise<Pick<User, 'uid' | 'email' | 'displayName'> | null> {
    const usersRef = ref(database, 'users');
    const emailQuery = query(usersRef, orderByChild('email'), equalTo(email));
    const snapshot = await get(emailQuery);

    if (snapshot.exists()) {
      const users = snapshot.val();
      const userId = Object.keys(users)[0];
      const userData = users[userId];
      
      // 이메일과 displayName만 반환 (민감한 정보 제외)
      return {
        uid: userId,
        email: userData.email,
        displayName: userData.displayName,
      };
    }

    return null;
  }

  /**
   * 사용자 정보 조회
   */
  async getUser(userId: string): Promise<User | null> {
    return await this.getUserDEK(userId);
  }

  // ========== Partner 관리 ==========

  /**
   * 배우자 정보 업데이트
   */
  async updatePartnerInfo(
    userId: string,
    partnerInfo: PartnerInfo
  ): Promise<void> {
    const partnerRef = ref(database, `users/${userId}/partner`);
    await set(partnerRef, partnerInfo);
  }

  /**
   * 배우자 정보 삭제
   */
  async removePartnerInfo(userId: string): Promise<void> {
    const partnerRef = ref(database, `users/${userId}/partner`);
    await remove(partnerRef);
  }

  /**
   * 배우자 정보 조회
   */
  async getPartnerInfo(userId: string): Promise<PartnerInfo | null> {
    const partnerRef = ref(database, `users/${userId}/partner`);
    const snapshot = await get(partnerRef);

    if (snapshot.exists()) {
      return snapshot.val() as PartnerInfo;
    }

    return null;
  }

  // ========== Pending Shared DEK 관리 ==========

  /**
   * Pending Shared DEK 저장 (배우자 초대 시)
   */
  async savePendingSharedDEK(
    userId: string,
    sharedDek: string,
    inviterUserId: string
  ): Promise<void> {
    const pendingRef = ref(database, `pending_shared_deks/${userId}`);
    const pendingData: PendingSharedDEK = {
      sharedDEK: sharedDek,
      inviterUserId,
      createdAt: getCurrentDateTime(),
    };

    await set(pendingRef, pendingData);
  }

  /**
   * Pending Shared DEK 조회
   */
  async getPendingSharedDEK(userId: string): Promise<PendingSharedDEK | null> {
    const pendingRef = ref(database, `pending_shared_deks/${userId}`);
    const snapshot = await get(pendingRef);

    if (snapshot.exists()) {
      return snapshot.val() as PendingSharedDEK;
    }

    return null;
  }

  /**
   * Pending Shared DEK 삭제
   */
  async deletePendingSharedDEK(userId: string): Promise<void> {
    const pendingRef = ref(database, `pending_shared_deks/${userId}`);
    await remove(pendingRef);
  }

  /**
   * 배우자의 Shared DEK 업데이트 (배우자가 로그인 후 처리)
   */
  async updatePartnerSharedDEK(
    userId: string,
    encryptedSharedDEK: { value: string; timestamp: string }
  ): Promise<void> {
    const sharedDekRef = ref(
      database,
      `users/${userId}/partner/encryptedSharedDEK`
    );
    await set(sharedDekRef, encryptedSharedDEK);
  }

  // ========== Topic 관리 ==========

  /**
   * 사용자의 모든 Topic 조회
   */
  async getTopics(userId: string): Promise<Topic[]> {
    const topicsRef = ref(database, `topics/${userId}`);
    const snapshot = await get(topicsRef);

    if (!snapshot.exists()) {
      return [];
    }

    const topicsData = snapshot.val();
    const topics: Topic[] = [];

    Object.keys(topicsData).forEach((firebaseKey) => {
      const topicData = topicsData[firebaseKey];
      topics.push({
        ...topicData,
        firebaseKey,
        userId,
      } as Topic);
    });

    return topics;
  }

  /**
   * 특정 Topic 조회
   */
  async getTopicById(userId: string, topicId: string): Promise<Topic | null> {
    const topicRef = ref(database, `topics/${userId}/${topicId}`);
    const snapshot = await get(topicRef);

    if (snapshot.exists()) {
      return {
        ...snapshot.val(),
        firebaseKey: topicId,
        userId,
      } as Topic;
    }

    return null;
  }

  /**
   * Topic 생성
   */
  async createTopic(
    userId: string,
    request: CreateTopicRequest
  ): Promise<string> {
    const topicsRef = ref(database, `topics/${userId}`);
    const newTopicRef = push(topicsRef);

    if (!newTopicRef.key) {
      throw new Error('Failed to generate topic key');
    }

    const topicData = {
      subject: request.subject,
      topicDate: request.topicDate,
      isActive: true,
      createdAt: getCurrentDateTime(),
      updatedAt: getCurrentDateTime(),
    };

    await set(newTopicRef, topicData);
    return newTopicRef.key;
  }

  /**
   * Topic 수정
   */
  async updateTopic(
    userId: string,
    topicId: string,
    request: UpdateTopicRequest
  ): Promise<void> {
    const topicRef = ref(database, `topics/${userId}/${topicId}`);
    const updateData: Partial<CreateTopicRequest> & { updatedAt: string } = {
      updatedAt: getCurrentDateTime(),
    };

    if (request.subject !== undefined) {
      updateData.subject = request.subject;
    }
    if (request.topicDate !== undefined) {
      updateData.topicDate = request.topicDate;
    }

    await update(topicRef, updateData);
  }

  /**
   * Topic 삭제 (Hard Delete)
   */
  async deleteTopic(userId: string, topicId: string): Promise<void> {
    const topicRef = ref(database, `topics/${userId}/${topicId}`);
    await remove(topicRef);
  }

  // ========== TenTen 관리 ==========

  /**
   * 사용자의 모든 TenTen 조회
   */
  async getTenTens(userId: string): Promise<TenTen[]> {
    const tentensRef = ref(database, `tentens/${userId}`);
    const snapshot = await get(tentensRef);

    if (!snapshot.exists()) {
      return [];
    }

    const tentensData = snapshot.val();
    const tentens: TenTen[] = [];

    Object.keys(tentensData).forEach((firebaseKey) => {
      const tentenData = tentensData[firebaseKey];
      tentens.push({
        ...tentenData,
        firebaseKey,
        userId,
      } as TenTen);
    });

    return tentens;
  }

  /**
   * 특정 Topic의 모든 TenTen 조회
   */
  async getTenTensByTopic(
    userId: string,
    topicId: string
  ): Promise<TenTen[]> {
    const tentensRef = ref(database, `tentens/${userId}`);
    const snapshot = await get(tentensRef);

    if (!snapshot.exists()) {
      return [];
    }

    const tentensData = snapshot.val();
    const tentens: TenTen[] = [];

    Object.keys(tentensData).forEach((firebaseKey) => {
      const tentenData = tentensData[firebaseKey];
      if (tentenData.topicId === topicId) {
        tentens.push({
          ...tentenData,
          firebaseKey,
          userId,
        } as TenTen);
      }
    });

    return tentens;
  }

  /**
   * 특정 TenTen 조회
   */
  async getTenTenById(
    userId: string,
    tenTenId: string
  ): Promise<TenTen | null> {
    const tentenRef = ref(database, `tentens/${userId}/${tenTenId}`);
    const snapshot = await get(tentenRef);

    if (snapshot.exists()) {
      return {
        ...snapshot.val(),
        firebaseKey: tenTenId,
        userId,
      } as TenTen;
    }

    return null;
  }

  /**
   * TenTen 생성
   */
  async createTenTen(
    userId: string,
    userName: string,
    topicSubject: string,
    request: CreateTenTenRequest
  ): Promise<string> {
    const tentensRef = ref(database, `tentens/${userId}`);
    const newTenTenRef = push(tentensRef);

    if (!newTenTenRef.key) {
      throw new Error('Failed to generate tenten key');
    }

    const tentenData = {
      content: request.content, // 암호화된 내용 (Base64)
      topicId: request.topicId,
      userName,
      topicSubject,
      isEncrypted: true,
      encryptionType: request.encryptionType,
      isReadByPartner: false,
      createdAt: getCurrentDateTime(),
      updatedAt: getCurrentDateTime(),
    };

    await set(newTenTenRef, tentenData);
    return newTenTenRef.key;
  }

  /**
   * TenTen 수정
   */
  async updateTenTen(
    userId: string,
    tenTenId: string,
    request: UpdateTenTenRequest
  ): Promise<void> {
    const tentenRef = ref(database, `tentens/${userId}/${tenTenId}`);
    const updateData: Partial<CreateTenTenRequest> & { updatedAt: string } = {
      updatedAt: getCurrentDateTime(),
    };

    if (request.content !== undefined) {
      updateData.content = request.content; // 암호화된 내용 (Base64)
    }

    await update(tentenRef, updateData);
  }

  /**
   * TenTen 삭제
   */
  async deleteTenTen(userId: string, tenTenId: string): Promise<void> {
    const tentenRef = ref(database, `tentens/${userId}/${tenTenId}`);
    await remove(tentenRef);
  }

  /**
   * TenTen 읽음 표시 업데이트
   */
  async markTenTenAsRead(
    userId: string,
    tenTenId: string
  ): Promise<void> {
    const tentenRef = ref(database, `tentens/${userId}/${tenTenId}`);
    await update(tentenRef, {
      isReadByPartner: true,
      readByPartnerAt: getCurrentDateTime(),
      updatedAt: getCurrentDateTime(),
    });
  }

  // ========== 유틸리티 ==========

  /**
   * 데이터베이스 참조 가져오기
   */
  getRef(path: string) {
    return ref(database, path);
  }

  /**
   * 자식 참조 가져오기
   */
  getChildRef(parentRef: ReturnType<typeof ref>, childPath: string) {
    return child(parentRef, childPath);
  }
}

// 싱글톤 인스턴스
export const firebaseDataService = new FirebaseDataService();
export default firebaseDataService;

