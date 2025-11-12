# MeTenTen 웹앱 기능 명세서

## 1. 앱 개요

### 1.1 목적
**MeTenTen**은 Marriage Encounter의 10&10 프로그램을 기반으로 한 부부 소통 웹 애플리케이션입니다. 10분간 편지를 쓰고 10분간 대화를 나누는 시간을 통해 부부가 서로의 마음을 깊이 이해할 수 있도록 돕습니다.

### 1.2 핵심 가치
- **프라이버시**: End-to-End 암호화로 완벽한 데이터 보안
- **소통 촉진**: 구조화된 주제와 시간 관리로 효과적인 대화 유도
- **영적 성장**: 기도문과 감정 표현을 통한 영적 발전 지원
- **파트너 연결**: 안전한 데이터 공유를 통한 부부 간 친밀감 증진

## 2. 전체 기능 목록

### 2.1 인증 및 사용자 관리
- 회원가입 (이메일/비밀번호)
- 로그인/로그아웃
- 비밀번호 변경
- 자동 로그인

### 2.2 10&10 작성 및 관리
- 주제(Topic) CRUD
- TenTen 작성/수정/삭제
- 10분 타이머 기능
- 월별 필터링
- 작성 완료 상태 표시

### 2.3 배우자 연결 및 공유
- 배우자 초대 (이메일)
- 배우자 연결 관리
- 배우자의 10&10 조회
- 실시간 동기화

### 2.4 느낌 표현
- 4가지 감정 카테고리 (기쁨, 두려움, 분노, 슬픔)
- 감정 표현 예시 제공
- 사용자 정의 예시 추가/삭제

### 2.5 기도문
- 부부를 위한 기도문 모음
- 기도문 보기/복사
- 카테고리별 정리

### 2.6 설정
- 내 정보 조회
- 비밀번호 변경
- 배우자 관리
- 로그아웃

## 3. 기능 상세 설명

### 3.1 인증 시스템

#### 3.1.1 회원가입
**입력 필드:**
- 이메일 (필수, 이메일 형식)
- 비밀번호 (필수, 최소 6자)
- 이름/표시명 (필수)

**프로세스:**
1. Firebase Authentication에 사용자 등록
2. 랜덤 256-bit DEK(Data Encryption Key) 생성
3. DEK를 사용자 비밀번호로 암호화 (PBKDF2, 100,000 iterations)
4. 암호화된 DEK를 Firebase Realtime Database에 저장
5. 사용자 정보 저장 (email, displayName, encryptedDEK, createdAt)

#### 3.1.2 로그인
**입력 필드:**
- 이메일
- 비밀번호

**프로세스:**
1. Firebase Authentication으로 인증
2. 인증 토큰 발급 및 저장
3. Firebase에서 암호화된 DEK 조회
4. 비밀번호로 DEK 복호화
5. DEK를 메모리에 로드 (데이터 암호화/복호화에 사용)
6. Pending Shared DEK 확인 및 처리 (배우자 초대 수락)
7. 홈 화면으로 이동

#### 3.1.3 비밀번호 변경
**입력 필드:**
- 현재 비밀번호
- 새 비밀번호
- 새 비밀번호 확인

**프로세스:**
1. 현재 비밀번호로 재인증
2. 기존 DEK를 현재 비밀번호로 복호화
3. DEK를 새 비밀번호로 재암호화
4. 재암호화된 DEK를 Firebase에 저장
5. ✅ 모든 10&10 데이터는 그대로 유지 (DEK만 변경)

### 3.2 10&10 시스템

#### 3.2.1 Topic(주제) 관리

**Topic 데이터 모델:**
```typescript
interface Topic {
  id: number;              // 로컬 ID
  firebaseKey: string;     // Firebase 고유 키
  subject: string;         // 주제
  description?: string;    // 설명
  topicDate: Date;         // 주제 날짜
  createdAt: Date;         // 생성일
  updatedAt?: Date;        // 수정일
  isActive: boolean;       // 활성화 여부
}
```

**기능:**
- 새 주제 추가 (날짜, 주제 입력)
- 주제 수정
- 주제 삭제 (Soft Delete - isActive = false)
- 월별 필터링
- 작성 완료 상태 표시

#### 3.2.2 TenTen 작성

**TenTen 데이터 모델:**
```typescript
interface TenTen {
  id: number;
  firebaseKey: string;
  content: string;           // 암호화된 내용
  topicId: string;           // 연결된 Topic의 firebaseKey
  createdAt: Date;
  updatedAt?: Date;
  userId: number;
  userName: string;
  topicSubject: string;
  isEncrypted: boolean;      // 암호화 여부
  encryptionType: string;    // "personal" or "shared"
  isReadByPartner: boolean;
  readByPartnerAt?: Date;
}
```

**기능:**
- 주제 선택 후 10&10 작성
- 10분 타이머 (카운트다운)
- 실시간 자동 저장 (선택적)
- 작성 완료 후 저장
- 기존 TenTen 수정/삭제
- 주제별 TenTen 목록 조회

**암호화:**
- Personal Mode: 개인 DEK로 암호화 (본인만 읽기 가능)
- Shared Mode: 공유 DEK로 암호화 (배우자도 읽기 가능)

### 3.3 배우자 연결 시스템

#### 3.3.1 배우자 초대

**프로세스:**
1. 사용자가 배우자 이메일 입력
2. 배우자 이메일로 사용자 검색
3. 공유 DEK 생성 (256-bit random)
4. 공유 DEK를 초대자 비밀번호로 암호화 → 초대자 계정에 저장
5. 공유 DEK를 Pending Shared DEK 노드에 저장 (배우자가 로그인 시 자동 처리)
6. 양쪽 사용자에 Partner 정보 저장

#### 3.3.2 Partner 데이터 모델

```typescript
interface PartnerInfo {
  partnerId: string;           // 배우자의 Firebase UID
  partnerEmail: string;
  partnerDisplayName: string;
  connectedAt: string;         // ISO 8601
  encryptedSharedDEK?: {      // 공유 DEK (각자 비밀번호로 암호화)
    value: string;
    timestamp: string;
  };
  sharedTopicId?: string;      // 공유 Topic ID (향후 사용)
}
```

#### 3.3.3 배우자 10&10 조회

**기능:**
- 배우자가 작성 완료한 Topic 목록 조회
- 특정 Topic의 TenTen 읽기
- 실시간 동기화 (배우자가 작성하면 즉시 반영)
- 읽음 표시 (향후 기능)

### 3.4 느낌 표현 시스템

**4가지 카테고리:**
1. 😊 기쁨 (Joy)
2. 😰 두려움 (Fear)
3. 😠 분노 (Anger)
4. 😢 슬픔 (Sadness)

**FeelingExample 데이터 모델:**
```typescript
interface FeelingExample {
  id: number;
  category: string;        // "joy", "fear", "anger", "sadness"
  subCategory: string;     // 세부 감정 (예: "행복한", "불안한")
  description: string;     // 감정 설명
  isDefault: boolean;      // 기본 예시 여부
  createdAt: Date;
}
```

**기능:**
- 카테고리별 감정 예시 조회
- 아코디언 방식 UI (카테고리 펼치기/접기)
- 사용자 정의 예시 추가
- 기본 예시는 삭제 불가

### 3.5 기도문 시스템

**Prayer 데이터 모델:**
```typescript
interface Prayer {
  id: number;
  title: string;
  content: string;
  category: string;
  tags?: string;
  isFavorite: boolean;
  viewCount: number;
  createdAt: Date;
  updatedAt?: Date;
}
```

**기능:**
- 기도문 목록 조회
- 기도문 상세 보기
- 클립보드 복사
- 즐겨찾기 (향후 기능)

### 3.6 설정 페이지

**기능:**
1. **내 정보 관리**
   - 이름, 이메일 표시
   - 비밀번호 변경

2. **배우자 관리**
   - 배우자 초대 (이메일)
   - 연결된 배우자 정보 표시
   - 배우자 연결 해제

3. **계정 관리**
   - 로그아웃

## 4. 보안 요구사항

### 4.1 DEK 암호화 시스템

**구조:**
```
User Password → PBKDF2 (100,000 iterations, SHA-256) → KEK (Key Encryption Key)
KEK + Random 256-bit DEK → AES-256-CBC → Encrypted DEK → Firebase Storage

TenTen Content + DEK → AES-256-CBC + Random IV → Encrypted Content → Firebase Storage
```

**특징:**
- **End-to-End 암호화**: Firebase 관리자도 데이터 읽기 불가
- **비밀번호 변경 가능**: DEK만 재암호화 (데이터는 유지)
- **개인/공유 암호화**: Personal DEK와 Shared DEK 분리 관리

### 4.2 Firebase Security Rules

**users 노드:**
- 읽기: 본인 또는 연결된 배우자만
- 쓰기: 본인만
- 필수 필드: Email, EncryptedDEK, CreatedAt

**topics 노드:**
- 읽기/쓰기: 본인만
- 필수 필드: subject, topicDate, createdAt, isActive

**tentens 노드:**
- 읽기: 본인 또는 연결된 배우자 (공유 Topic인 경우)
- 쓰기: 본인만
- 필수 필드: content, createdAt, topicId, isEncrypted

**pending_shared_deks 노드:**
- 읽기/쓰기: 본인만
- 필수 필드: sharedDEK, inviterUserId, createdAt

### 4.3 인증 토큰 관리

- Firebase Authentication ID Token 사용
- 모든 Firebase 요청에 토큰 자동 첨부
- 토큰 만료 시 자동 갱신
- 로그아웃 시 토큰 삭제

## 5. 데이터 모델 전체

### 5.1 Firebase Realtime Database 구조

```json
{
  "users": {
    "{userId}": {
      "email": "user@example.com",
      "displayName": "사용자 이름",
      "encryptedDEK": "Base64EncodedEncryptedDEK",
      "createdAt": "2025-01-01T00:00:00.000Z",
      "updatedAt": "2025-01-02T00:00:00.000Z",
      "partner": {
        "partnerId": "{partnerUserId}",
        "partnerEmail": "partner@example.com",
        "partnerDisplayName": "배우자 이름",
        "connectedAt": "2025-01-01T00:00:00.000Z",
        "encryptedSharedDEK": {
          "value": "Base64EncodedEncryptedSharedDEK",
          "timestamp": "2025-01-01T00:00:00.000Z"
        }
      }
    }
  },
  "topics": {
    "{userId}": {
      "{topicId}": {
        "subject": "주제 제목",
        "description": "주제 설명",
        "topicDate": "2025-01-01",
        "createdAt": "2025-01-01T00:00:00.000Z",
        "updatedAt": "2025-01-02T00:00:00.000Z",
        "isActive": true
      }
    }
  },
  "tentens": {
    "{userId}": {
      "{tentenId}": {
        "topicId": "{topicId}",
        "content": "Base64EncodedEncryptedContent",
        "isEncrypted": true,
        "encryptionType": "personal",
        "createdAt": "2025-01-01T00:00:00.000Z",
        "updatedAt": "2025-01-02T00:00:00.000Z"
      }
    }
  },
  "pending_shared_deks": {
    "{userId}": {
      "sharedDEK": "Base64EncodedSharedDEK",
      "inviterUserId": "{inviterUserId}",
      "createdAt": "2025-01-01T00:00:00.000Z"
    }
  }
}
```

### 5.2 클라이언트 데이터 모델

모든 모델은 위에 설명된 TypeScript 인터페이스 참조

## 6. UI/UX 요구사항

### 6.1 디자인 원칙
- **간결함**: 복잡하지 않은 직관적인 UI
- **따뜻함**: 부부 소통 앱에 어울리는 따뜻한 색상과 디자인
- **접근성**: 모든 연령대가 쉽게 사용할 수 있는 인터페이스
- **반응형**: 모바일, 태블릿, 데스크톱 모두 지원

### 6.2 주요 화면

#### 6.2.1 홈 화면
- 환영 메시지
- 6개 카드 레이아웃:
  1. 나의 10&10
  2. 배우자 10&10
  3. 느낌 표현
  4. 주요 기도문
  5. 설정
- Marriage Encounter 소개

#### 6.2.2 10&10 페이지
- 월별 필터 (드롭다운)
- 새 주제 추가 버튼
- 주제 목록 (작성 완료 상태 표시)
- 주제 클릭 → TenTen 작성/조회 모달

#### 6.2.3 TenTen 작성 모달
- 주제 정보 표시
- 10분 타이머 (카운트다운)
- 텍스트 에리어 (자동 크기 조정)
- 저장/취소 버튼
- 수정/삭제 옵션 (기존 TenTen)

#### 6.2.4 배우자 10&10 페이지
- 배우자 이름 표시
- 작성 완료된 Topic 목록
- Topic 클릭 → 배우자 TenTen 읽기
- 실시간 업데이트 표시

#### 6.2.5 느낌 표현 페이지
- 4개 카테고리 아코디언
- 각 카테고리별 예시 목록
- 예시 추가 버튼
- 기본 예시는 삭제 불가

#### 6.2.6 기도문 페이지
- 기도문 목록
- 아코디언 방식 확장/축소
- 복사 버튼

#### 6.2.7 설정 페이지
- 3개 섹션:
  1. 내 정보 관리 (비밀번호 변경)
  2. 배우자 관리 (초대/연결 해제)
  3. 계정 관리 (로그아웃)

### 6.3 네비게이션
- 상단 네비게이션 바:
  - 로고
  - 사용자 정보 (이름, 이메일)
  - 로그아웃 버튼
- Breadcrumb 네비게이션 (현재 페이지 표시)
- 뒤로 가기 기능

### 6.4 색상 테마 (예시)
```css
:root {
  --primary-color: #FF6B9D;      /* 핑크 - 사랑과 소통 */
  --secondary-color: #C44569;    /* 진한 핑크 - 강조 */
  --background-color: #FFF5F7;   /* 연한 핑크 - 배경 */
  --text-color: #2C3E50;         /* 다크 그레이 - 텍스트 */
  --success-color: #52C41A;      /* 그린 - 성공 */
  --danger-color: #F5222D;       /* 레드 - 경고/삭제 */
  --border-color: #E8E8E8;       /* 라이트 그레이 - 테두리 */
}
```

## 7. API/서비스 구조

### 7.1 서비스 레이어

**1. AuthService**
- `signUp(email, password, name)`
- `signIn(email, password)`
- `signOut()`
- `changePassword(currentPassword, newPassword)`
- `getCurrentUser()`

**2. EncryptionService**
- `generateRandomDEK()`
- `encryptDEK(dek, email, password)`
- `decryptDEK(encryptedDEK, email, password)`
- `encrypt(plaintext, dek)`
- `decrypt(ciphertext, dek)`
- `generateSharedDEK()`
- `setDEK(dek)`
- `setSharedDEK(sharedDek)`
- `clearKeys()`

**3. FirebaseDataService**
- User DEK:
  - `saveUserDEK(userId, email, displayName, encryptedDEK)`
  - `getUserDEK(userId)`
- Partner:
  - `getUserByEmail(email)`
  - `getUser(userId)`
  - `updatePartnerInfo(userId, partnerInfo)`
  - `removePartnerInfo(userId)`
- Pending Shared DEK:
  - `savePendingSharedDEK(userId, sharedDek, inviterUserId)`
  - `getPendingSharedDEK(userId)`
  - `deletePendingSharedDEK(userId)`
  - `updatePartnerSharedDEK(userId, encryptedSharedDEK)`

**4. TopicService**
- `getTopics(userId)`
- `getTopicById(userId, topicId)`
- `createTopic(userId, request)`
- `updateTopic(userId, topicId, request)`
- `deleteTopic(userId, topicId)`

**5. TenTenService**
- `getTenTens(userId)`
- `getTenTensByTopic(userId, topicId)`
- `getTenTenById(userId, tenTenId)`
- `createTenTen(userId, request, encryptionType)`
- `updateTenTen(userId, tenTenId, request)`
- `deleteTenTen(userId, tenTenId)`

**6. PartnerService**
- `invitePartner(partnerEmail, myPassword)`
- `disconnectPartner()`
- `getPartnerInfo()`
- `getPartnerCompletedTopics()`
- `getPartnerTenTens(topicId)`

**7. FeelingExampleService**
- `getAllExamples()`
- `getExamplesByCategory(category)`
- `addExample(example)`
- `deleteExample(id)`

**8. PrayerService**
- `getAllPrayers()`
- `getPrayerById(id)`
- `toggleFavorite(id)`

## 8. 비기능 요구사항

### 8.1 성능
- 초기 로딩 시간: < 3초
- 페이지 전환: < 1초
- Firebase 데이터 로딩: < 2초
- 암호화/복호화: < 500ms (일반 TenTen 기준)

### 8.2 보안
- HTTPS 필수
- XSS 방지
- CSRF 방지
- Firebase Security Rules 강제 적용
- 민감한 데이터는 메모리에서 즉시 삭제

### 8.3 호환성
- **브라우저**: Chrome, Firefox, Safari, Edge (최신 2개 버전)
- **모바일**: iOS Safari, Android Chrome
- **해상도**: 320px ~ 2560px

### 8.4 접근성
- WCAG 2.1 Level AA 준수
- 키보드 네비게이션 지원
- 스크린 리더 지원
- 적절한 색상 대비

## 9. 향후 개발 계획 (v1.3+)

### 9.1 우선순위 높음
- 읽음 확인 기능
- 알림 시스템 (10&10 작성 리마인더)
- 통계 및 분석 (월별, 연도별)

### 9.2 우선순위 중간
- 복구 키 백업 시스템
- 오프라인 지원
- 모바일 앱 (PWA)

### 9.3 우선순위 낮음
- 테마 변경 (다크 모드)
- 다국어 지원
- 감정 트렌드 분석

## 10. 웹앱 전환 시 고려사항

### 10.1 MAUI → 웹 전환 차이점
1. **플랫폼 특정 코드 제거**
   - `SecureStorage` → 브라우저 `localStorage` + 암호화
   - `Platforms/` 폴더 불필요

2. **UI 프레임워크 변경**
   - Blazor Hybrid → Blazor WebAssembly 또는 React/Vue/Angular
   - `.razor` 파일 → `.tsx`/`.vue` 파일

3. **네비게이션**
   - MAUI NavigationManager → 웹 라우터 (React Router, Vue Router)

4. **Firebase SDK**
   - .NET Firebase SDK → JavaScript Firebase SDK
   - 동일한 구조이므로 마이그레이션 용이

5. **암호화**
   - C# `System.Security.Cryptography` → Web Crypto API
   - 동일한 AES-256-CBC, PBKDF2 사용 가능

### 10.2 권장 기술 스택
- **프론트엔드**: React + TypeScript + Vite
- **UI 라이브러리**: Material-UI 또는 Ant Design
- **상태 관리**: Zustand 또는 Redux Toolkit
- **라우팅**: React Router v6
- **백엔드**: Firebase (Authentication + Realtime Database)
- **배포**: Vercel, Netlify, 또는 Firebase Hosting

---

이 명세서는 MeTenTen MAUI 앱의 현재 구현을 기반으로 작성되었으며, 웹앱 개발 시 참고 문서로 사용할 수 있습니다.


