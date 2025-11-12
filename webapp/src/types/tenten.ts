/**
 * TenTen 관련 타입 정의
 */

import { FirebaseEntity, EncryptionType } from './common';

export interface TenTen extends FirebaseEntity {
  id: number; // 로컬 ID
  content: string; // 암호화된 내용 (Base64)
  topicId: string; // 연결된 Topic의 firebaseKey
  userId: string; // Firebase UID
  userName: string; // 작성자 이름
  topicSubject: string; // Topic 제목 (캐싱용)
  isEncrypted: boolean; // 암호화 여부
  encryptionType: EncryptionType; // "personal" or "shared"
  isReadByPartner: boolean; // 배우자가 읽었는지 여부
  readByPartnerAt?: string; // 배우자가 읽은 시간
}

export interface CreateTenTenRequest {
  content: string; // 평문
  topicId: string;
  encryptionType: EncryptionType;
}

export interface UpdateTenTenRequest {
  content: string; // 평문
}

export interface TenTenWithTopic extends TenTen {
  topicDate: string;
  topicDescription?: string;
}

export interface DecryptedTenTen extends Omit<TenTen, 'content'> {
  content: string; // 복호화된 평문
}

