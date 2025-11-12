/**
 * Topic 관련 타입 정의
 */

import { FirebaseEntity } from './common';

export interface Topic extends FirebaseEntity {
  id: number; // 로컬 ID
  subject: string; // 주제
  description?: string; // 설명
  topicDate: string; // YYYY-MM-DD format
  isActive: boolean; // 활성화 여부
  userId: string; // Firebase UID
  hasMyTenTen?: boolean; // 내가 작성했는지 여부 (UI용)
  hasPartnerTenTen?: boolean; // 배우자가 작성했는지 여부 (UI용)
}

export interface CreateTopicRequest {
  subject: string;
  description?: string;
  topicDate: string; // YYYY-MM-DD format
}

export interface UpdateTopicRequest {
  subject?: string;
  description?: string;
  topicDate?: string;
}

export interface TopicFilter {
  year?: number;
  month?: number;
  isActive?: boolean;
}

export interface TopicListItem extends Topic {
  tenTenCount: number;
}

