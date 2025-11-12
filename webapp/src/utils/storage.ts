/**
 * 로컬 스토리지 헬퍼 함수
 */

/**
 * 로컬 스토리지에 데이터 저장
 */
export const setStorage = <T>(key: string, value: T): void => {
  try {
    const serialized = JSON.stringify(value);
    localStorage.setItem(key, serialized);
  } catch (error) {
    console.error('Failed to save to localStorage:', error);
  }
};

/**
 * 로컬 스토리지에서 데이터 조회
 */
export const getStorage = <T>(key: string): T | null => {
  try {
    const serialized = localStorage.getItem(key);
    if (serialized === null) {
      return null;
    }
    return JSON.parse(serialized) as T;
  } catch (error) {
    console.error('Failed to load from localStorage:', error);
    return null;
  }
};

/**
 * 로컬 스토리지에서 데이터 삭제
 */
export const removeStorage = (key: string): void => {
  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.error('Failed to remove from localStorage:', error);
  }
};

/**
 * 로컬 스토리지 전체 삭제
 */
export const clearStorage = (): void => {
  try {
    localStorage.clear();
  } catch (error) {
    console.error('Failed to clear localStorage:', error);
  }
};

/**
 * 특정 키가 로컬 스토리지에 존재하는지 확인
 */
export const hasStorage = (key: string): boolean => {
  return localStorage.getItem(key) !== null;
};

