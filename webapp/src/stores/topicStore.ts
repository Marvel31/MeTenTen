/**
 * Topic Zustand Store
 */

import { create } from 'zustand';
import { topicService } from '@services/TopicService';
import type { Topic, TopicFilter } from '../types/topic';

interface TopicState {
  topics: Topic[];
  selectedTopic: Topic | null;
  loading: boolean;
  error: string | null;
  filter: TopicFilter | null;
  sortBy: 'date' | 'createdAt';
  sortOrder: 'asc' | 'desc';
  currentPage: number;
  pageSize: number;
  total: number;
}

interface TopicActions {
  // 데이터 로딩
  loadTopics: (userId: string) => Promise<void>;
  loadTopicById: (userId: string, topicId: string) => Promise<void>;
  refreshTopics: (userId: string) => Promise<void>;

  // 필터 및 정렬
  setFilter: (filter: TopicFilter | null) => void;
  setSortBy: (sortBy: 'date' | 'createdAt') => void;
  setSortOrder: (sortOrder: 'asc' | 'desc') => void;
  setCurrentPage: (page: number) => void;
  setPageSize: (pageSize: number) => void;

  // 선택
  selectTopic: (topic: Topic | null) => void;
  clearSelection: () => void;

  // 상태 초기화
  reset: () => void;
}

const initialState: TopicState = {
  topics: [],
  selectedTopic: null,
  loading: false,
  error: null,
  filter: null,
  sortBy: 'date',
  sortOrder: 'desc',
  currentPage: 1,
  pageSize: 10,
  total: 0,
};

export const useTopicStore = create<TopicState & TopicActions>((set, get) => ({
  ...initialState,

  loadTopics: async (userId: string) => {
    set({ loading: true, error: null });

    try {
      const { filter, sortBy, sortOrder } = get();
      const topics = await topicService.getTopics(
        userId,
        filter || undefined,
        sortBy,
        sortOrder
      );

      set({
        topics,
        total: topics.length,
        loading: false,
      });
    } catch (error) {
      set({
        error:
          error instanceof Error
            ? error.message
            : '주제 목록을 불러오는데 실패했습니다.',
        loading: false,
      });
    }
  },

  loadTopicById: async (userId: string, topicId: string) => {
    set({ loading: true, error: null });

    try {
      const topic = await topicService.getTopicById(userId, topicId);
      if (topic) {
        set({
          selectedTopic: topic,
          loading: false,
        });
      } else {
        set({
          error: '주제를 찾을 수 없습니다.',
          loading: false,
        });
      }
    } catch (error) {
      set({
        error:
          error instanceof Error
            ? error.message
            : '주제를 불러오는데 실패했습니다.',
        loading: false,
      });
    }
  },

  refreshTopics: async (userId: string) => {
    await get().loadTopics(userId);
  },

  setFilter: (filter: TopicFilter | null) => {
    set({ filter, currentPage: 1 });
  },

  setSortBy: (sortBy: 'date' | 'createdAt') => {
    set({ sortBy });
  },

  setSortOrder: (sortOrder: 'asc' | 'desc') => {
    set({ sortOrder });
  },

  setCurrentPage: (page: number) => {
    set({ currentPage: page });
  },

  setPageSize: (pageSize: number) => {
    set({ pageSize, currentPage: 1 });
  },

  selectTopic: (topic: Topic | null) => {
    set({ selectedTopic: topic });
  },

  clearSelection: () => {
    set({ selectedTopic: null });
  },

  reset: () => {
    set(initialState);
  },
}));

