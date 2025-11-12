/**
 * 느낌 표현 관련 타입 정의
 */

import type { BaseEntity } from './common';

export type FeelingCategory = 'joy' | 'fear' | 'anger' | 'sadness';

export interface FeelingExample extends BaseEntity {
  id: string;
  category: FeelingCategory;
  subCategory: string; // 세부 감정 (예: "행복한", "불안한")
  description: string; // 감정 설명
  isDefault: boolean; // 기본 예시 여부
}

export interface CreateFeelingExampleRequest {
  category: FeelingCategory;
  subCategory: string;
  description: string;
}

export interface FeelingCategoryInfo {
  category: FeelingCategory;
  displayName: string;
  emoji: string;
  examples: FeelingExample[];
}

