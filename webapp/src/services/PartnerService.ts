/**
 * 배우자 연결 서비스
 * 배우자 초대, 연결 해제, TenTen 공유 등의 비즈니스 로직
 */

import { firebaseDataService } from './FirebaseDataService';
import { encryptionService } from './EncryptionService';
import { authService } from './AuthService';
import { useAuthStore } from '@stores/authStore';
import type { PartnerInfo } from '../types/user';
import { getCurrentDateTime } from '@utils/date';
import { ERROR_MESSAGES, SUCCESS_MESSAGES } from '@utils/constants';

class PartnerService {
  /**
   * 배우자 초대
   * @param partnerEmail 배우자 이메일
   * @param myPassword 본인 비밀번호 (공유 DEK 암호화용)
   */
  async invitePartner(
    partnerEmail: string,
    myPassword: string
  ): Promise<void> {
    const currentUser = authService.getCurrentUser();
    if (!currentUser) {
      throw new Error('로그인이 필요합니다.');
    }

    const userId = currentUser.uid;
    const myEmail = currentUser.email;
    if (!myEmail) {
      throw new Error('이메일 정보를 찾을 수 없습니다.');
    }

    // 1. 본인 이메일과 동일한지 확인
    if (partnerEmail.toLowerCase() === myEmail.toLowerCase()) {
      throw new Error('본인 이메일은 입력할 수 없습니다.');
    }

    // 2. 배우자 검색
    const partnerUser = await firebaseDataService.getUserByEmail(partnerEmail);
    if (!partnerUser) {
      throw new Error('해당 이메일로 가입된 사용자를 찾을 수 없습니다.');
    }

    // 3. 이미 연결되어 있는지 확인 (양쪽 모두 확인)
    const existingPartnerInfo = await firebaseDataService.getPartnerInfo(userId);
    const partnerPartnerInfo = await firebaseDataService.getPartnerInfo(
      partnerUser.uid
    );

    // 본인 쪽에 배우자 정보가 있고, 배우자와 매칭되는 경우
    if (existingPartnerInfo && existingPartnerInfo.partnerId === partnerUser.uid) {
      // 배우자 쪽도 확인: 양쪽 모두 정상적으로 연결되어 있으면 이미 연결됨
      if (partnerPartnerInfo && partnerPartnerInfo.partnerId === userId) {
        throw new Error('이미 연결된 배우자입니다.');
      } else {
        // 불완전한 상태: 본인 쪽만 연결 정보가 있음 (이전 시도 중 실패)
        // 기존 정보를 삭제하고 다시 초대 진행
        await firebaseDataService.removePartnerInfo(userId);
      }
    }

    // 배우자 쪽에 다른 사람과 연결되어 있는 경우
    if (partnerPartnerInfo && partnerPartnerInfo.partnerId !== userId) {
      throw new Error('해당 사용자는 이미 다른 사람과 연결되어 있습니다.');
    }

    // 배우자 쪽에 본인과의 불완전한 연결 정보가 있는 경우 정리
    if (partnerPartnerInfo && partnerPartnerInfo.partnerId === userId) {
      // Pending Shared DEK도 확인하여 정리
      const pendingDEK = await firebaseDataService.getPendingSharedDEK(partnerUser.uid);
      if (!pendingDEK || pendingDEK.inviterUserId !== userId) {
        // 불완전한 상태: 배우자 쪽 연결 정보는 있지만 Pending이 없거나 다른 사람의 것
        // 정리 후 다시 진행
        await firebaseDataService.removePartnerInfo(partnerUser.uid);
      }
    }

    // 5. 공유 DEK 생성
    const sharedDEK = await encryptionService.generateSharedDEK();

    // 6. 공유 DEK를 Base64 문자열로 export (Pending에 저장할 때 사용)
    const sharedDEKBase64 = await encryptionService.exportKeyToBase64(sharedDEK);

    // 7. 공유 DEK를 본인 비밀번호로 암호화 (본인 계정에 저장)
    const myEncryptedSharedDEK = await encryptionService.encryptDEK(
      sharedDEK,
      myEmail,
      myPassword
    );

    // 8. 본인 계정에 Partner 정보 및 암호화된 Shared DEK 준비
    const myPartnerInfo: PartnerInfo = {
      partnerId: partnerUser.uid,
      partnerEmail: partnerUser.email,
      partnerDisplayName: partnerUser.displayName,
      connectedAt: getCurrentDateTime(),
      encryptedSharedDEK: {
        value: myEncryptedSharedDEK,
        timestamp: getCurrentDateTime(),
      },
    };

    try {
      // 9. 본인 계정에 Partner 정보 및 암호화된 Shared DEK 저장
      await firebaseDataService.updatePartnerInfo(userId, myPartnerInfo);

      // 10. 배우자 계정에 Pending Shared DEK 저장 (Base64 문자열로 저장, 배우자가 로그인 시 자신의 비밀번호로 재암호화)
      await firebaseDataService.savePendingSharedDEK(
        partnerUser.uid,
        sharedDEKBase64, // Base64로 인코딩된 공유 DEK (평문)
        userId
      );

      // 11. 배우자 계정에 Partner 정보 저장 (encryptedSharedDEK는 배우자가 로그인할 때 설정됨)
      const partnerPartnerInfoForSave: PartnerInfo = {
        partnerId: userId,
        partnerEmail: myEmail,
        partnerDisplayName: currentUser.displayName || '사용자',
        connectedAt: getCurrentDateTime(),
      };

      await firebaseDataService.updatePartnerInfo(
        partnerUser.uid,
        partnerPartnerInfoForSave
      );
    } catch (error) {
      // 에러 발생 시 롤백: 본인 쪽 저장된 데이터만 정리 (배우자 쪽은 권한 없음)
      console.error('Invite partner error, rolling back:', error);
      
      try {
        // 본인 쪽 정보만 삭제 (본인 권한으로 가능)
        await firebaseDataService.removePartnerInfo(userId);
      } catch (rollbackError) {
        console.error('Rollback error (own data):', rollbackError);
        // 본인 쪽 롤백 실패는 로그만 남김
      }
      
      // 배우자 쪽 데이터는 본인 권한으로 삭제 불가능
      // 배우자가 다음에 로그인할 때 불완전한 상태를 감지하고 정리할 수 있도록
      // 또는 수동으로 정리해야 함
      console.warn('Partner data may remain incomplete. Partner should clean up on next login.');
      
      throw error;
    }

    // 12. 본인 인증 상태 업데이트 (Shared DEK 메모리에 로드)
    encryptionService.setSharedDEK(sharedDEK);
    useAuthStore.getState().setHasSharedDEK(true);

    // 13. 사용자 정보 업데이트
    const user = useAuthStore.getState().user;
    if (user) {
      useAuthStore.getState().setUser({
        ...user,
        partner: myPartnerInfo,
      });
    }
  }

  /**
   * 배우자 연결 해제
   */
  async disconnectPartner(): Promise<void> {
    const currentUser = authService.getCurrentUser();
    if (!currentUser) {
      throw new Error('로그인이 필요합니다.');
    }

    const userId = currentUser.uid;

    // 1. 본인 배우자 정보 조회
    const myPartnerInfo = await firebaseDataService.getPartnerInfo(userId);
    if (!myPartnerInfo) {
      throw new Error('연결된 배우자가 없습니다.');
    }

    const partnerId = myPartnerInfo.partnerId;

    // 2. 본인 배우자 정보 삭제
    await firebaseDataService.removePartnerInfo(userId);

    // 3. 배우자 계정의 배우자 정보 삭제
    await firebaseDataService.removePartnerInfo(partnerId);

    // 4. Shared DEK 메모리에서 삭제
    encryptionService.clearKeys();
    useAuthStore.getState().setHasSharedDEK(false);

    // 5. 사용자 정보 업데이트
    const user = useAuthStore.getState().user;
    if (user) {
      useAuthStore.getState().setUser({
        ...user,
        partner: undefined,
      });
    }
  }

  /**
   * 배우자 정보 조회
   */
  async getPartnerInfo(): Promise<PartnerInfo | null> {
    const currentUser = authService.getCurrentUser();
    if (!currentUser) {
      return null;
    }

    return await firebaseDataService.getPartnerInfo(currentUser.uid);
  }

  /**
   * 배우자가 작성 완료한 Topic 목록 조회
   */
  async getPartnerCompletedTopics(): Promise<string[]> {
    const currentUser = authService.getCurrentUser();
    if (!currentUser) {
      throw new Error('로그인이 필요합니다.');
    }

    const myPartnerInfo = await firebaseDataService.getPartnerInfo(
      currentUser.uid
    );
    if (!myPartnerInfo) {
      return [];
    }

    // 배우자의 모든 TenTen 조회
    const partnerTenTens = await firebaseDataService.getTenTens(
      myPartnerInfo.partnerId
    );

    // Shared 타입 TenTen의 topicId만 추출
    const topicIds = partnerTenTens
      .filter((tenten) => tenten.encryptionType === 'shared')
      .map((tenten) => tenten.topicId);

    // 중복 제거
    return Array.from(new Set(topicIds));
  }

  /**
   * 배우자의 특정 Topic의 TenTen 조회 (복호화 포함)
   */
  async getPartnerTenTens(topicId: string) {
    const currentUser = authService.getCurrentUser();
    if (!currentUser) {
      throw new Error('로그인이 필요합니다.');
    }

    const myPartnerInfo = await firebaseDataService.getPartnerInfo(
      currentUser.uid
    );
    if (!myPartnerInfo) {
      throw new Error('연결된 배우자가 없습니다.');
    }

    if (!encryptionService.hasSharedDEK()) {
      throw new Error('Shared DEK가 설정되지 않았습니다.');
    }

    // 배우자의 해당 Topic의 TenTen 조회
    const partnerTenTens = await firebaseDataService.getTenTensByTopic(
      myPartnerInfo.partnerId,
      topicId
    );

    // Shared 타입 TenTen만 필터링 및 복호화
    const decryptedTenTens = await Promise.all(
      partnerTenTens
        .filter((tenten) => tenten.encryptionType === 'shared')
        .map(async (tenten) => {
          const decryptedContent =
            await encryptionService.decryptWithSharedDEK(tenten.content);

          return {
            ...tenten,
            content: decryptedContent,
          };
        })
    );

    return decryptedTenTens;
  }

}

// 싱글톤 인스턴스
export const partnerService = new PartnerService();
export default partnerService;

