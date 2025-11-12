/**
 * 공통 타입 정의
 */

export type Timestamp = string; // ISO 8601 format

export interface BaseEntity {
  createdAt: Timestamp;
  updatedAt?: Timestamp;
}

export interface FirebaseEntity extends BaseEntity {
  firebaseKey: string;
}

export type EncryptionType = 'personal' | 'shared';

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

export interface PaginationParams {
  page: number;
  pageSize: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}

