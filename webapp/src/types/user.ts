/**
 * 사용자 관련 타입 정의
 */

import type { Timestamp } from './common';

export interface User {
  uid: string; // Firebase UID
  email: string;
  displayName: string;
  encryptedDEK: string; // Base64 encoded
  createdAt: Timestamp;
  updatedAt?: Timestamp;
  partner?: PartnerInfo;
}

export interface PartnerInfo {
  partnerId: string; // 배우자의 Firebase UID
  partnerEmail: string;
  partnerDisplayName: string;
  connectedAt: Timestamp;
  encryptedSharedDEK?: {
    value: string; // Base64 encoded
    timestamp: Timestamp;
  };
  sharedTopicId?: string; // 공유 Topic ID (향후 사용)
}

export interface PendingSharedDEK {
  sharedDEK: string; // Base64 encoded
  inviterUserId: string;
  createdAt: Timestamp;
}

export interface UserProfile {
  uid: string;
  email: string;
  displayName: string;
  hasPartner: boolean;
  partnerName?: string;
}

export interface SignUpRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

