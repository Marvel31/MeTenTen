/**
 * 기도문 관련 타입 정의
 */

import { BaseEntity } from './common';

export interface Prayer extends BaseEntity {
  id: string;
  title: string;
  content: string;
  category: string;
  tags?: string[];
  isFavorite: boolean;
  viewCount: number;
}

export interface PrayerCategory {
  id: string;
  name: string;
  description?: string;
  prayers: Prayer[];
}

