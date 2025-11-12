/**
 * 폼 유효성 검증 함수
 */

import { ERROR_MESSAGES } from './constants';

/**
 * 이메일 유효성 검증
 */
export const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

/**
 * 비밀번호 유효성 검증 (최소 6자)
 */
export const isValidPassword = (password: string): boolean => {
  return password.length >= 6;
};

/**
 * 필수 필드 검증
 */
export const isRequired = (value: string): boolean => {
  return value.trim().length > 0;
};

/**
 * 이메일 검증 및 에러 메시지 반환
 */
export const validateEmail = (email: string): string | null => {
  if (!isRequired(email)) {
    return '이메일을 입력해주세요.';
  }
  if (!isValidEmail(email)) {
    return ERROR_MESSAGES.INVALID_EMAIL;
  }
  return null;
};

/**
 * 비밀번호 검증 및 에러 메시지 반환
 */
export const validatePassword = (password: string): string | null => {
  if (!isRequired(password)) {
    return '비밀번호를 입력해주세요.';
  }
  if (!isValidPassword(password)) {
    return ERROR_MESSAGES.INVALID_PASSWORD;
  }
  return null;
};

/**
 * 비밀번호 확인 검증
 */
export const validatePasswordConfirm = (
  password: string,
  passwordConfirm: string
): string | null => {
  if (!isRequired(passwordConfirm)) {
    return '비밀번호 확인을 입력해주세요.';
  }
  if (password !== passwordConfirm) {
    return ERROR_MESSAGES.PASSWORD_MISMATCH;
  }
  return null;
};

/**
 * 이름 검증
 */
export const validateDisplayName = (name: string): string | null => {
  if (!isRequired(name)) {
    return '이름을 입력해주세요.';
  }
  if (name.length < 2) {
    return '이름은 최소 2자 이상이어야 합니다.';
  }
  return null;
};

/**
 * 주제 제목 검증
 */
export const validateTopicSubject = (subject: string): string | null => {
  if (!isRequired(subject)) {
    return '주제를 입력해주세요.';
  }
  if (subject.length > 200) {
    return '주제는 200자 이하로 입력해주세요.';
  }
  return null;
};

/**
 * TenTen 내용 검증
 */
export const validateTenTenContent = (content: string): string | null => {
  if (!isRequired(content)) {
    return '내용을 입력해주세요.';
  }
  if (content.length > 10000) {
    return '내용은 10,000자 이하로 입력해주세요.';
  }
  return null;
};

