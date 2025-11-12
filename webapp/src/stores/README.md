# Stores

Zustand를 사용한 전역 상태 관리 스토어입니다.

## 예정된 스토어

- **authStore.ts**: 인증 상태 관리 (user, isAuthenticated, DEK)
- **uiStore.ts**: UI 상태 관리 (로딩, 에러, 모달 상태)
- **topicStore.ts**: Topic 데이터 캐싱 (선택적)
- **tentenStore.ts**: TenTen 데이터 캐싱 (선택적)

## 사용 예시

```typescript
import { useAuthStore } from '@stores/authStore';

const MyComponent = () => {
  const { user, isAuthenticated, signIn, signOut } = useAuthStore();
  
  // ...
};
```

