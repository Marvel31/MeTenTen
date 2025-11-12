# Utils

유틸리티 함수들을 모아둔 폴더입니다.

## 예정된 유틸리티

- **crypto.ts**: 암호화 관련 헬퍼 함수
- **date.ts**: 날짜 포맷팅 및 변환
- **validation.ts**: 폼 유효성 검증
- **storage.ts**: 로컬 스토리지 헬퍼
- **constants.ts**: 상수 정의
- **helpers.ts**: 기타 헬퍼 함수

## 사용 예시

```typescript
import { formatDate, isValidEmail } from '@utils/helpers';

const formattedDate = formatDate(new Date(), 'yyyy-MM-dd');
const isValid = isValidEmail('user@example.com');
```

