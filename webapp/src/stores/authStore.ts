/**
 * 인증 상태 관리 스토어 (Zustand)
 */

import { create } from 'zustand';
import type { User } from '@types/user';

interface AuthState {
  // 상태
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // DEK는 메모리에만 저장 (EncryptionService에서 관리)
  // 여기서는 DEK가 설정되었는지 여부만 추적
  hasDEK: boolean;
  hasSharedDEK: boolean;

  // 액션
  setUser: (user: User | null) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  setHasDEK: (has: boolean) => void;
  setHasSharedDEK: (has: boolean) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  // 초기 상태
  user: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  hasDEK: false,
  hasSharedDEK: false,

  // 액션
  setUser: (user) =>
    set({
      user,
      isAuthenticated: user !== null,
      error: null,
    }),

  setLoading: (loading) =>
    set({
      isLoading: loading,
    }),

  setError: (error) =>
    set({
      error,
      isLoading: false,
    }),

  setHasDEK: (has) =>
    set({
      hasDEK: has,
    }),

  setHasSharedDEK: (has) =>
    set({
      hasSharedDEK: has,
    }),

  clearAuth: () =>
    set({
      user: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,
      hasDEK: false,
      hasSharedDEK: false,
    }),
}));

export default useAuthStore;

