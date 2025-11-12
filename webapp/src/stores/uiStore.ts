/**
 * UI 상태 관리 스토어 (Zustand)
 */

import { create } from 'zustand';

interface ModalState {
  isOpen: boolean;
  type: string | null;
  data?: unknown;
}

interface ToastState {
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
  duration?: number;
}

interface UIState {
  // 전역 로딩
  isGlobalLoading: boolean;
  loadingMessage?: string;

  // 모달
  modal: ModalState;

  // 토스트
  toast: ToastState | null;

  // 액션
  setGlobalLoading: (loading: boolean, message?: string) => void;
  openModal: (type: string, data?: unknown) => void;
  closeModal: () => void;
  showToast: (toast: ToastState) => void;
  hideToast: () => void;
}

export const useUIStore = create<UIState>((set) => ({
  // 초기 상태
  isGlobalLoading: false,
  loadingMessage: undefined,

  modal: {
    isOpen: false,
    type: null,
    data: undefined,
  },

  toast: null,

  // 액션
  setGlobalLoading: (loading, message) =>
    set({
      isGlobalLoading: loading,
      loadingMessage: message,
    }),

  openModal: (type, data) =>
    set({
      modal: {
        isOpen: true,
        type,
        data,
      },
    }),

  closeModal: () =>
    set({
      modal: {
        isOpen: false,
        type: null,
        data: undefined,
      },
    }),

  showToast: (toast) =>
    set({
      toast,
    }),

  hideToast: () =>
    set({
      toast: null,
    }),
}));

export default useUIStore;

