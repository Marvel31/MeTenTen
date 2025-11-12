# Hooks

커스텀 React 훅들을 모아둔 폴더입니다.

## 예정된 훅

- **useAuth**: 인증 관련 훅
- **useTimer**: 10분 타이머 훅
- **useFirebase**: Firebase 실시간 데이터 구독 훅
- **useEncryption**: 암호화/복호화 훅
- **useLocalStorage**: 로컬 스토리지 관리 훅
- **useDebounce**: 디바운스 훅
- **useAsync**: 비동기 작업 관리 훅

## 사용 예시

```typescript
import { useTimer } from '@hooks/useTimer';

const MyComponent = () => {
  const { minutes, seconds, isRunning, start, pause, reset } = useTimer(10);
  
  // ...
};
```

