/**
 * 날짜 관련 유틸리티 함수
 */

import { format, parseISO, isValid } from 'date-fns';
import { ko } from 'date-fns/locale';
import { DATE_FORMAT, DATETIME_FORMAT, DISPLAY_DATE_FORMAT } from './constants';

/**
 * Date를 ISO 8601 문자열로 변환
 */
export const toISOString = (date: Date): string => {
  return date.toISOString();
};

/**
 * ISO 8601 문자열을 Date로 변환
 */
export const fromISOString = (isoString: string): Date => {
  return parseISO(isoString);
};

/**
 * Date를 YYYY-MM-DD 형식으로 포맷
 */
export const formatDate = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  if (!isValid(dateObj)) {
    return '';
  }
  return format(dateObj, DATE_FORMAT);
};

/**
 * Date를 YYYY-MM-DD HH:mm:ss 형식으로 포맷
 */
export const formatDateTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  if (!isValid(dateObj)) {
    return '';
  }
  return format(dateObj, DATETIME_FORMAT);
};

/**
 * Date를 HH:mm:ss 형식으로 포맷 (시간만)
 */
export const formatTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  if (!isValid(dateObj)) {
    return '';
  }
  return format(dateObj, 'HH:mm:ss');
};

/**
 * Date를 한국어 표시 형식으로 포맷 (YYYY년 MM월 DD일)
 */
export const formatDisplayDate = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  if (!isValid(dateObj)) {
    return '';
  }
  return format(dateObj, DISPLAY_DATE_FORMAT, { locale: ko });
};

/**
 * 현재 날짜를 YYYY-MM-DD 형식으로 반환
 */
export const getCurrentDate = (): string => {
  return formatDate(new Date());
};

/**
 * 현재 날짜와 시간을 ISO 8601 형식으로 반환
 */
export const getCurrentDateTime = (): string => {
  return toISOString(new Date());
};

/**
 * 년/월을 기준으로 필터링용 날짜 범위 생성
 */
export const getMonthRange = (year: number, month: number) => {
  const startDate = new Date(year, month - 1, 1);
  const endDate = new Date(year, month, 0, 23, 59, 59, 999);

  return {
    start: toISOString(startDate),
    end: toISOString(endDate),
    startDate: formatDate(startDate),
    endDate: formatDate(endDate),
  };
};

/**
 * YYYY-MM-DD 형식의 문자열이 유효한지 검증
 */
export const isValidDateString = (dateString: string): boolean => {
  const regex = /^\d{4}-\d{2}-\d{2}$/;
  if (!regex.test(dateString)) {
    return false;
  }
  const date = parseISO(dateString);
  return isValid(date);
};

/**
 * 두 날짜 사이의 차이 (일 단위)
 */
export const getDaysDifference = (date1: Date, date2: Date): number => {
  const diffTime = Math.abs(date2.getTime() - date1.getTime());
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
};

/**
 * 상대적 시간 표시 (예: "방금 전", "3일 전")
 */
export const getRelativeTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  const now = new Date();
  const diffSeconds = Math.floor((now.getTime() - dateObj.getTime()) / 1000);

  if (diffSeconds < 60) {
    return '방금 전';
  }

  const diffMinutes = Math.floor(diffSeconds / 60);
  if (diffMinutes < 60) {
    return `${diffMinutes}분 전`;
  }

  const diffHours = Math.floor(diffMinutes / 60);
  if (diffHours < 24) {
    return `${diffHours}시간 전`;
  }

  const diffDays = Math.floor(diffHours / 24);
  if (diffDays < 30) {
    return `${diffDays}일 전`;
  }

  const diffMonths = Math.floor(diffDays / 30);
  if (diffMonths < 12) {
    return `${diffMonths}개월 전`;
  }

  const diffYears = Math.floor(diffMonths / 12);
  return `${diffYears}년 전`;
};

