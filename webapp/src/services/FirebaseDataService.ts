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
} from 'firebase/database';
import type { User, PartnerInfo, PendingSharedDEK } from '@types/user';
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
   * 이메일로 사용자 검색
   */
  async getUserByEmail(email: string): Promise<User | null> {
    const usersRef = ref(database, 'users');
    const emailQuery = query(usersRef, orderByChild('email'), equalTo(email));
    const snapshot = await get(emailQuery);

    if (snapshot.exists()) {
      const users = snapshot.val();
      const userId = Object.keys(users)[0];
      return {
        uid: userId,
        ...users[userId],
      } as User;
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

