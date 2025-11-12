/**
 * 인증 서비스
 * Firebase Authentication 연동 및 DEK 관리
 */

import { auth } from '@config/firebase';
import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signOut as firebaseSignOut,
  updatePassword,
  reauthenticateWithCredential,
  EmailAuthProvider,
  User as FirebaseUser,
} from 'firebase/auth';
import { encryptionService } from './EncryptionService';
import { firebaseDataService } from './FirebaseDataService';
import { useAuthStore } from '@stores/authStore';
import {
  SignUpRequest,
  SignInRequest,
  ChangePasswordRequest,
  User,
} from '@types/user';
import { ERROR_MESSAGES } from '@utils/constants';

class AuthService {
  /**
   * 회원가입
   */
  async signUp(request: SignUpRequest): Promise<User> {
    try {
      // 1. Firebase Authentication에 사용자 등록
      const userCredential = await createUserWithEmailAndPassword(
        auth,
        request.email,
        request.password
      );

      const firebaseUser = userCredential.user;

      // 2. Random DEK 생성
      const dek = await encryptionService.generateRandomDEK();

      // 3. DEK를 사용자 비밀번호로 암호화
      const encryptedDEK = await encryptionService.encryptDEK(
        dek,
        request.email,
        request.password
      );

      // 4. 암호화된 DEK를 Firebase Realtime Database에 저장
      await firebaseDataService.saveUserDEK(
        firebaseUser.uid,
        request.email,
        request.displayName,
        encryptedDEK
      );

      // 5. DEK를 메모리에 로드
      encryptionService.setDEK(dek);

      // 6. 사용자 정보 반환
      const user: User = {
        uid: firebaseUser.uid,
        email: request.email,
        displayName: request.displayName,
        encryptedDEK,
        createdAt: new Date().toISOString(),
      };

      // 7. 인증 상태 업데이트
      useAuthStore.getState().setUser(user);
      useAuthStore.getState().setHasDEK(true);

      return user;
    } catch (error: unknown) {
      console.error('Sign up error:', error);
      throw this.handleAuthError(error);
    }
  }

  /**
   * 로그인
   */
  async signIn(request: SignInRequest): Promise<User> {
    try {
      // 1. Firebase Authentication으로 인증
      const userCredential = await signInWithEmailAndPassword(
        auth,
        request.email,
        request.password
      );

      const firebaseUser = userCredential.user;

      // 2. Firebase에서 사용자 데이터 조회
      const userData = await firebaseDataService.getUserDEK(firebaseUser.uid);
      if (!userData) {
        throw new Error(ERROR_MESSAGES.USER_NOT_FOUND);
      }

      // 3. DEK 복호화
      const dek = await encryptionService.decryptDEK(
        userData.encryptedDEK,
        request.email,
        request.password
      );

      // 4. DEK를 메모리에 로드
      encryptionService.setDEK(dek);

      // 5. Pending Shared DEK 확인 및 처리
      await this.processPendingSharedDEK(firebaseUser.uid, request.password);

      // 6. 배우자 정보 확인
      const partnerInfo = await firebaseDataService.getPartnerInfo(
        firebaseUser.uid
      );
      if (partnerInfo && partnerInfo.encryptedSharedDEK) {
        // 공유 DEK 복호화
        const sharedDek = await encryptionService.decryptDEK(
          partnerInfo.encryptedSharedDEK.value,
          request.email,
          request.password
        );
        encryptionService.setSharedDEK(sharedDek);
        useAuthStore.getState().setHasSharedDEK(true);
      }

      // 7. 사용자 정보 반환
      const user: User = {
        ...userData,
        partner: partnerInfo || undefined,
      };

      // 8. 인증 상태 업데이트
      useAuthStore.getState().setUser(user);
      useAuthStore.getState().setHasDEK(true);

      return user;
    } catch (error: unknown) {
      console.error('Sign in error:', error);
      throw this.handleAuthError(error);
    }
  }

  /**
   * 로그아웃
   */
  async signOut(): Promise<void> {
    try {
      // 1. DEK 메모리 삭제
      encryptionService.clearKeys();

      // 2. Firebase 로그아웃
      await firebaseSignOut(auth);

      // 3. 인증 상태 초기화
      useAuthStore.getState().clearAuth();
    } catch (error: unknown) {
      console.error('Sign out error:', error);
      throw this.handleAuthError(error);
    }
  }

  /**
   * 비밀번호 변경
   */
  async changePassword(request: ChangePasswordRequest): Promise<void> {
    try {
      const currentUser = auth.currentUser;
      if (!currentUser || !currentUser.email) {
        throw new Error(ERROR_MESSAGES.AUTH_FAILED);
      }

      // 1. 현재 비밀번호로 재인증
      const credential = EmailAuthProvider.credential(
        currentUser.email,
        request.currentPassword
      );
      await reauthenticateWithCredential(currentUser, credential);

      // 2. 사용자 데이터 조회
      const userData = await firebaseDataService.getUserDEK(currentUser.uid);
      if (!userData) {
        throw new Error(ERROR_MESSAGES.USER_NOT_FOUND);
      }

      // 3. 기존 DEK를 현재 비밀번호로 복호화
      const dek = await encryptionService.decryptDEK(
        userData.encryptedDEK,
        currentUser.email,
        request.currentPassword
      );

      // 4. DEK를 새 비밀번호로 재암호화
      const newEncryptedDEK = await encryptionService.encryptDEK(
        dek,
        currentUser.email,
        request.newPassword
      );

      // 5. Firebase Authentication 비밀번호 업데이트
      await updatePassword(currentUser, request.newPassword);

      // 6. 재암호화된 DEK를 Firebase에 저장
      await firebaseDataService.updateUserDEK(currentUser.uid, newEncryptedDEK);

      // 7. Shared DEK도 재암호화 (배우자가 있는 경우)
      const partnerInfo = await firebaseDataService.getPartnerInfo(
        currentUser.uid
      );
      if (partnerInfo && partnerInfo.encryptedSharedDEK) {
        const sharedDek = await encryptionService.decryptDEK(
          partnerInfo.encryptedSharedDEK.value,
          currentUser.email,
          request.currentPassword
        );

        const newEncryptedSharedDEK = await encryptionService.encryptDEK(
          sharedDek,
          currentUser.email,
          request.newPassword
        );

        await firebaseDataService.updatePartnerSharedDEK(currentUser.uid, {
          value: newEncryptedSharedDEK,
          timestamp: new Date().toISOString(),
        });
      }

      console.log('Password changed successfully');
    } catch (error: unknown) {
      console.error('Change password error:', error);
      throw this.handleAuthError(error);
    }
  }

  /**
   * 현재 로그인한 사용자 가져오기
   */
  getCurrentUser(): FirebaseUser | null {
    return auth.currentUser;
  }

  /**
   * Pending Shared DEK 처리 (로그인 시 자동 실행)
   */
  private async processPendingSharedDEK(
    userId: string,
    password: string
  ): Promise<void> {
    try {
      const pendingData = await firebaseDataService.getPendingSharedDEK(userId);
      if (!pendingData) {
        return; // Pending 데이터 없음
      }

      const currentUser = auth.currentUser;
      if (!currentUser || !currentUser.email) {
        return;
      }

      // 1. Pending Shared DEK를 CryptoKey로 import
      const sharedDek = await encryptionService.importKeyFromBase64(
        pendingData.sharedDEK
      );

      // 2. Shared DEK를 내 비밀번호로 암호화
      const encryptedSharedDEK = await encryptionService.encryptDEK(
        sharedDek,
        currentUser.email,
        password
      );

      // 3. 내 계정에 Shared DEK 저장
      await firebaseDataService.updatePartnerSharedDEK(userId, {
        value: encryptedSharedDEK,
        timestamp: new Date().toISOString(),
      });

      // 4. Shared DEK를 메모리에 로드
      encryptionService.setSharedDEK(sharedDek);
      useAuthStore.getState().setHasSharedDEK(true);

      // 5. Pending 데이터 삭제
      await firebaseDataService.deletePendingSharedDEK(userId);

      console.log('Pending Shared DEK processed successfully');
    } catch (error) {
      console.error('Process pending shared DEK error:', error);
      // 에러가 발생해도 로그인은 계속 진행
    }
  }

  /**
   * Firebase 인증 에러 처리
   */
  private handleAuthError(error: unknown): Error {
    if (error instanceof Error) {
      const firebaseError = error as { code?: string };
      
      switch (firebaseError.code) {
        case 'auth/email-already-in-use':
          return new Error(ERROR_MESSAGES.EMAIL_ALREADY_IN_USE);
        case 'auth/invalid-email':
          return new Error(ERROR_MESSAGES.INVALID_EMAIL);
        case 'auth/user-not-found':
          return new Error(ERROR_MESSAGES.USER_NOT_FOUND);
        case 'auth/wrong-password':
          return new Error(ERROR_MESSAGES.WRONG_PASSWORD);
        case 'auth/weak-password':
          return new Error(ERROR_MESSAGES.INVALID_PASSWORD);
        case 'auth/network-request-failed':
          return new Error(ERROR_MESSAGES.NETWORK_ERROR);
        default:
          return error;
      }
    }
    return new Error(ERROR_MESSAGES.UNKNOWN_ERROR);
  }
}

// 싱글톤 인스턴스
export const authService = new AuthService();
export default authService;

