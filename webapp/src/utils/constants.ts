/**
 * 상수 정의
 */

// 앱 정보
export const APP_NAME = import.meta.env.VITE_APP_NAME || '10&10';
export const APP_VERSION = import.meta.env.VITE_APP_VERSION || '1.0.0';

// 로컬 스토리지 키
export const STORAGE_KEYS = {
  AUTH_TOKEN: 'metenten_auth_token',
  USER_EMAIL: 'metenten_user_email',
  REMEMBER_ME: 'metenten_remember_me',
  FEELINGS_DATA: 'metenten_feelings_data',
  PRAYERS_DATA: 'metenten_prayers_data',
  CREDENTIALS: 'metenten_credentials',
} as const;

// 자격증명 설정
export const CREDENTIAL_EXPIRY_DAYS = 30;

// 타이머 설정
export const TIMER_DURATION_MINUTES = 10;
export const TIMER_DURATION_SECONDS = TIMER_DURATION_MINUTES * 60;

// 페이지네이션
export const DEFAULT_PAGE_SIZE = 20;

// 날짜 포맷
export const DATE_FORMAT = 'yyyy-MM-dd';
export const DATETIME_FORMAT = 'yyyy-MM-dd HH:mm:ss';
export const DISPLAY_DATE_FORMAT = 'yyyy년 MM월 dd일';

// 감정 카테고리
export const FEELING_CATEGORIES = {
  JOY: 'joy',
  FEAR: 'fear',
  ANGER: 'anger',
  SADNESS: 'sadness',
} as const;

export const FEELING_CATEGORY_LABELS = {
  [FEELING_CATEGORIES.JOY]: '기쁨',
  [FEELING_CATEGORIES.FEAR]: '두려움',
  [FEELING_CATEGORIES.ANGER]: '분노',
  [FEELING_CATEGORIES.SADNESS]: '슬픔',
} as const;

export const FEELING_CATEGORY_EMOJIS = {
  [FEELING_CATEGORIES.JOY]: '😊',
  [FEELING_CATEGORIES.FEAR]: '😰',
  [FEELING_CATEGORIES.ANGER]: '😠',
  [FEELING_CATEGORIES.SADNESS]: '😢',
} as const;

// 암호화 타입
export const ENCRYPTION_TYPES = {
  PERSONAL: 'personal',
  SHARED: 'shared',
} as const;

// 에러 메시지
export const ERROR_MESSAGES = {
  NETWORK_ERROR: '네트워크 오류가 발생했습니다.',
  AUTH_FAILED: '인증에 실패했습니다.',
  INVALID_EMAIL: '유효하지 않은 이메일 주소입니다.',
  INVALID_PASSWORD: '비밀번호는 최소 6자 이상이어야 합니다.',
  PASSWORD_MISMATCH: '비밀번호가 일치하지 않습니다.',
  USER_NOT_FOUND: '사용자를 찾을 수 없습니다.',
  EMAIL_ALREADY_IN_USE: '이미 사용 중인 이메일입니다.',
  WRONG_PASSWORD: '비밀번호가 올바르지 않습니다.',
  DEK_NOT_SET: 'DEK가 설정되지 않았습니다.',
  ENCRYPTION_FAILED: '암호화에 실패했습니다.',
  DECRYPTION_FAILED: '복호화에 실패했습니다.',
  LOAD_FAILED: '데이터를 불러오는데 실패했습니다.',
  DELETE_FAILED: '삭제에 실패했습니다.',
  UNKNOWN_ERROR: '알 수 없는 오류가 발생했습니다.',
} as const;

// 성공 메시지
export const SUCCESS_MESSAGES = {
  SIGNUP_SUCCESS: '회원가입이 완료되었습니다.',
  LOGIN_SUCCESS: '로그인되었습니다.',
  LOGOUT_SUCCESS: '로그아웃되었습니다.',
  PASSWORD_CHANGED: '비밀번호가 변경되었습니다.',
  TOPIC_CREATED: '주제가 생성되었습니다.',
  TOPIC_UPDATED: '주제가 수정되었습니다.',
  TOPIC_DELETED: '주제가 삭제되었습니다.',
  TENTEN_CREATED: '10&10이 작성되었습니다.',
  TENTEN_UPDATED: '10&10이 수정되었습니다.',
  TENTEN_SAVED: 'TenTen이 저장되었습니다.',
  TENTEN_DELETED: 'TenTen이 삭제되었습니다.',
  PARTNER_INVITED: '배우자 초대가 완료되었습니다.',
  PARTNER_DISCONNECTED: '배우자 연결이 해제되었습니다.',
  COPIED_TO_CLIPBOARD: '클립보드에 복사되었습니다.',
} as const;

