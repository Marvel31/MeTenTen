# MeTenTen 웹앱 구현 Todo 리스트

> 이 문서는 WEBAPP_SPECIFICATION.md를 기반으로 작성된 구현용 Todo 리스트입니다.

---

## Phase 0: 프로젝트 초기화 및 개발 환경 설정

### 0.1 프로젝트 생성 및 기본 설정
- [x] Vite + React + TypeScript 프로젝트 초기화
- [x] Git 저장소 초기화 및 .gitignore 설정
- [x] 프로젝트 폴더 구조 설계
  ```
  src/
    ├── components/        # 재사용 가능한 UI 컴포넌트
    ├── pages/            # 페이지 컴포넌트
    ├── services/         # 비즈니스 로직 서비스
    ├── hooks/            # 커스텀 React 훅
    ├── utils/            # 유틸리티 함수
    ├── types/            # TypeScript 타입 정의
    ├── stores/           # 상태 관리 (Zustand)
    ├── styles/           # 전역 스타일
    └── config/           # 설정 파일
  ```
- [x] ESLint, Prettier 설정
- [x] TypeScript 설정 (tsconfig.json 최적화)

### 0.2 의존성 패키지 설치
- [x] React Router v6 설치
- [x] UI 라이브러리 선택 및 설치 (Ant Design)
- [x] 상태 관리 라이브러리 설치 (Zustand)
- [x] Firebase SDK 설치 (firebase)
- [x] 유틸리티 라이브러리 (date-fns, uuid 등)
- [x] 개발 도구 (ESLint, Prettier 등)

### 0.3 환경 변수 설정
- [x] `.env` 파일 생성 및 Firebase 설정 추가
- [x] 환경 변수 타입 정의 (vite-env.d.ts)
- [x] `.env.example` 파일 생성 (템플릿)

---

## Phase 1: 핵심 인프라 구축

### 1.1 Firebase 초기화
- [ ] Firebase 프로젝트 생성 (Console)
- [ ] Firebase Authentication 활성화 (Email/Password)
- [ ] Firebase Realtime Database 생성 및 설정
- [x] Firebase 설정 파일 작성 (`src/config/firebase.ts`)
- [x] Firebase 초기화 코드 작성

### 1.2 TypeScript 타입 정의
- [x] User 인터페이스 정의 (`src/types/user.ts`)
- [x] Topic 인터페이스 정의 (`src/types/topic.ts`)
- [x] TenTen 인터페이스 정의 (`src/types/tenten.ts`)
- [x] PartnerInfo 인터페이스 정의 (`src/types/user.ts`)
- [x] FeelingExample 인터페이스 정의 (`src/types/feeling.ts`)
- [x] Prayer 인터페이스 정의 (`src/types/prayer.ts`)
- [x] 공통 타입 정의 (`src/types/common.ts`)

### 1.3 암호화 서비스 구현 (EncryptionService)
- [x] Web Crypto API 래퍼 함수 작성
- [x] PBKDF2 구현 (100,000 iterations, SHA-256)
- [x] AES-256-CBC 암호화/복호화 함수 구현
- [x] Random DEK 생성 함수 (256-bit)
- [x] DEK 암호화/복호화 함수 (비밀번호 기반)
- [x] Shared DEK 생성 및 관리 함수
- [x] 메모리 DEK 관리 (setDEK, setSharedDEK, clearKeys)
- [x] Base64 인코딩/디코딩 유틸리티
- [ ] 암호화 서비스 단위 테스트 작성

### 1.4 상태 관리 설정
- [x] Zustand 스토어 생성 (`src/stores/authStore.ts`)
- [x] 사용자 인증 상태 관리 (user, isAuthenticated)
- [x] DEK 상태 관리 (hasDEK, hasSharedDEK - 메모리에만 저장)
- [x] 로딩 및 에러 상태 관리 (authStore, uiStore)
- [x] 전역 상태 통합 export (`src/stores/index.ts`)

### 1.5 라우팅 설정
- [x] 라우트 경로 상수 정의 (`src/config/routes.ts`)
- [x] React Router 설정 (`src/App.tsx`)
- [x] 라우트 정의 (/, /login, /signup, /home, /tentens, /partner, /feelings, /prayers, /settings)
- [x] Protected Route 컴포넌트 작성 (인증 필요)
- [x] Public Route 컴포넌트 작성 (로그인 후 접근 불가)
- [x] 404 페이지 작성
- [x] 기본 페이지 컴포넌트 작성 (임시)

---

## Phase 2: 인증 시스템 구현

### 2.1 AuthService 구현
- [x] Firebase Authentication 연동
- [x] `signUp(email, password, name)` 구현
  - Firebase 사용자 등록
  - Random DEK 생성
  - DEK 암호화 및 Firebase 저장
- [x] `signIn(email, password)` 구현
  - Firebase 인증
  - 암호화된 DEK 조회
  - DEK 복호화 및 메모리 로드
  - Pending Shared DEK 확인 및 처리
- [x] `signOut()` 구현
  - DEK 메모리 삭제
  - Firebase 로그아웃
- [x] `changePassword(currentPassword, newPassword)` 구현
  - 재인증
  - DEK 복호화 → 재암호화
  - Firebase 저장
- [x] `getCurrentUser()` 구현

### 2.2 FirebaseDataService 구현 (User/DEK 관련)
- [x] `saveUserDEK(userId, email, displayName, encryptedDEK)` 구현
- [x] `getUserDEK(userId)` 구현
- [x] `updateUserDEK(userId, encryptedDEK)` 구현
- [x] `getUserByEmail(email)` 구현
- [x] `getUser(userId)` 구현
- [x] Partner 관련 메서드 구현
- [x] Pending Shared DEK 관리 메서드 구현

### 2.3 회원가입 UI 구현
- [x] 회원가입 페이지 레이아웃 (`src/pages/SignUp.tsx`)
- [x] 입력 필드 (이메일, 비밀번호, 비밀번호 확인, 이름)
- [x] 폼 유효성 검증 (이메일 형식, 비밀번호 최소 6자, 비밀번호 일치)
- [x] 회원가입 버튼 및 로딩 상태
- [x] 에러 메시지 표시
- [x] 로그인 페이지 링크
- [x] 보안 안내 메시지 추가

### 2.4 로그인 UI 구현
- [x] 로그인 페이지 레이아웃 (`src/pages/Login.tsx`)
- [x] 입력 필드 (이메일, 비밀번호)
- [x] 이메일 기억하기 체크박스
- [x] 로그인 버튼 및 로딩 상태
- [x] 에러 메시지 표시
- [x] 회원가입 페이지 링크
- [x] 앱 로고 및 소개 문구 추가

### 2.5 비밀번호 변경 UI 구현
- [x] 비밀번호 변경 모달 (`src/components/ChangePasswordModal.tsx`)
- [x] 입력 필드 (현재 비밀번호, 새 비밀번호, 새 비밀번호 확인)
- [x] 폼 유효성 검증
- [x] 변경 버튼 및 로딩 상태
- [x] 성공/실패 메시지
- [x] 주의사항 안내 추가

### 2.6 인증 흐름 테스트
- [ ] 회원가입 → 로그인 → 로그아웃 테스트 (사용자 테스트 필요)
- [ ] 비밀번호 변경 테스트 (사용자 테스트 필요)
- [ ] DEK 암호화/복호화 검증
- [ ] 토큰 갱신 테스트

---

## Phase 3: 10&10 시스템 구현

### 3.1 FirebaseDataService 구현 (Topic)
- [ ] `getTopics(userId)` 구현
- [ ] `getTopicById(userId, topicId)` 구현
- [ ] `createTopic(userId, request)` 구현
- [ ] `updateTopic(userId, topicId, request)` 구현
- [ ] `deleteTopic(userId, topicId)` 구현 (Soft Delete)

### 3.2 TopicService 구현
- [ ] Topic CRUD 비즈니스 로직 구현
- [ ] 월별 필터링 로직 구현
- [ ] Topic 정렬 로직 (날짜 기준)
- [ ] Topic 완료 상태 확인 로직 (TenTen 존재 여부)

### 3.3 FirebaseDataService 구현 (TenTen)
- [ ] `getTenTens(userId)` 구현
- [ ] `getTenTensByTopic(userId, topicId)` 구현
- [ ] `getTenTenById(userId, tenTenId)` 구현
- [ ] `createTenTen(userId, request)` 구현
- [ ] `updateTenTen(userId, tenTenId, request)` 구현
- [ ] `deleteTenTen(userId, tenTenId)` 구현

### 3.4 TenTenService 구현
- [ ] TenTen CRUD 비즈니스 로직 구현
- [ ] 암호화 타입 선택 로직 (personal/shared)
- [ ] Personal DEK로 암호화/복호화
- [ ] Shared DEK로 암호화/복호화 (배우자 연결 시)
- [ ] 실시간 자동 저장 로직 (선택적)

### 3.5 나의 10&10 페이지 UI 구현
- [ ] 페이지 레이아웃 (`src/pages/MyTenTens.tsx`)
- [ ] 월별 필터 드롭다운
- [ ] 새 주제 추가 버튼
- [ ] Topic 목록 카드 (작성 완료 상태 표시)
- [ ] Topic 클릭 시 TenTen 작성 모달 열기

### 3.6 Topic 추가/수정 모달 구현
- [ ] 모달 컴포넌트 (`src/components/TopicModal.tsx`)
- [ ] 입력 필드 (주제, 설명, 날짜)
- [ ] 폼 유효성 검증
- [ ] 저장/취소 버튼
- [ ] 수정 모드 지원

### 3.7 TenTen 작성/수정 모달 구현
- [ ] 모달 컴포넌트 (`src/components/TenTenModal.tsx`)
- [ ] 주제 정보 표시
- [ ] 10분 타이머 구현 (카운트다운)
- [ ] 텍스트 에리어 (자동 크기 조정)
- [ ] 암호화 모드 선택 (Personal/Shared)
- [ ] 저장/취소 버튼
- [ ] 수정/삭제 옵션 (기존 TenTen)
- [ ] 실시간 자동 저장 (선택적)

### 3.8 타이머 컴포넌트 구현
- [ ] 10분 카운트다운 타이머 (`src/components/Timer.tsx`)
- [ ] 시작/일시정지/재시작 버튼
- [ ] 시간 표시 (MM:SS)
- [ ] 타이머 완료 알림
- [ ] 타이머 상태 관리

### 3.9 10&10 시스템 테스트
- [ ] Topic CRUD 테스트
- [ ] TenTen CRUD 테스트
- [ ] 암호화/복호화 검증
- [ ] 월별 필터링 테스트
- [ ] 타이머 기능 테스트

---

## Phase 4: 배우자 연결 시스템 구현

### 4.1 FirebaseDataService 구현 (Partner)
- [ ] `updatePartnerInfo(userId, partnerInfo)` 구현
- [ ] `removePartnerInfo(userId)` 구현
- [ ] `savePendingSharedDEK(userId, sharedDek, inviterUserId)` 구현
- [ ] `getPendingSharedDEK(userId)` 구현
- [ ] `deletePendingSharedDEK(userId)` 구현
- [ ] `updatePartnerSharedDEK(userId, encryptedSharedDEK)` 구현

### 4.2 PartnerService 구현
- [ ] `invitePartner(partnerEmail, myPassword)` 구현
  - 배우자 검색
  - 공유 DEK 생성
  - 공유 DEK 암호화 및 저장
  - Pending Shared DEK 저장
  - Partner 정보 양쪽 저장
- [ ] `disconnectPartner()` 구현
- [ ] `getPartnerInfo()` 구현
- [ ] `getPartnerCompletedTopics()` 구현
- [ ] `getPartnerTenTens(topicId)` 구현
- [ ] Pending Shared DEK 자동 처리 로직 (로그인 시)

### 4.3 배우자 초대 UI 구현
- [ ] 배우자 초대 모달 (`src/components/InvitePartner.tsx`)
- [ ] 배우자 이메일 입력 필드
- [ ] 비밀번호 재확인 (공유 DEK 암호화)
- [ ] 초대 버튼 및 로딩 상태
- [ ] 성공/실패 메시지

### 4.4 배우자 10&10 페이지 UI 구현
- [ ] 페이지 레이아웃 (`src/pages/PartnerTenTens.tsx`)
- [ ] 배우자 정보 표시
- [ ] 작성 완료된 Topic 목록
- [ ] Topic 클릭 시 배우자 TenTen 읽기 모달
- [ ] 실시간 업데이트 표시

### 4.5 배우자 TenTen 읽기 모달 구현
- [x] 모달 컴포넌트 (`src/components/PartnerTenTenView.tsx`)
- [x] 주제 정보 표시
- [x] 배우자 TenTen 내용 표시 (복호화)
- [x] 읽음 표시 (향후 기능 준비)
- [x] 닫기 버튼

### 4.6 배우자 연결 관리 UI 구현
- [x] 설정 페이지에 배우자 정보 표시
- [x] 배우자 연결 해제 버튼
- [x] 연결 해제 확인 다이얼로그

### 4.7 배우자 연결 시스템 테스트
- [ ] 배우자 초대 테스트
- [ ] 공유 DEK 생성 및 저장 검증
- [ ] Pending Shared DEK 자동 처리 테스트
- [ ] 배우자 TenTen 조회 테스트
- [ ] 배우자 연결 해제 테스트

---

## Phase 5: 보조 기능 구현

### 5.1 FeelingExampleService 구현
- [x] 기본 감정 예시 데이터 준비 (JSON)
- [x] `getAllExamples()` 구현 (로컬 스토리지)
- [x] `getExamplesByCategory(category)` 구현
- [x] `addExample(example)` 구현
- [x] `deleteExample(id)` 구현 (사용자 정의 예시만)
- [x] 초기 데이터 로드 로직

### 5.2 느낌 표현 페이지 UI 구현
- [x] 페이지 레이아웃 (`src/pages/Feelings.tsx`)
- [x] 4개 카테고리 아코디언 (기쁨, 두려움, 분노, 슬픔)
- [x] 각 카테고리별 예시 목록
- [x] 예시 추가 버튼 및 입력 필드
- [x] 사용자 정의 예시 삭제 버튼
- [x] 기본 예시는 삭제 불가 처리

### 5.3 PrayerService 구현
- [x] 기본 기도문 데이터 준비 (JSON)
- [x] `getAllPrayers()` 구현 (로컬 스토리지)
- [x] `getPrayerById(id)` 구현
- [x] `toggleFavorite(id)` 구현 (향후 기능)
- [x] 카테고리별 필터링 로직

### 5.4 기도문 페이지 UI 구현
- [x] 페이지 레이아웃 (`src/pages/Prayers.tsx`)
- [x] 기도문 목록 (아코디언 방식)
- [x] 기도문 상세 보기
- [x] 클립보드 복사 버튼
- [x] 복사 성공 토스트 메시지
- [x] 즐겨찾기 표시 (향후 기능 준비)

### 5.5 보조 기능 테스트
- [ ] 감정 예시 CRUD 테스트
- [ ] 기도문 조회 테스트
- [ ] 클립보드 복사 테스트

---

## Phase 6: UI/UX 완성 및 공통 컴포넌트

### 6.1 홈 페이지 구현
- [ ] 페이지 레이아웃 (`src/pages/Home.tsx`)
- [ ] 환영 메시지
- [ ] 6개 카드 레이아웃
  1. 나의 10&10
  2. 배우자 10&10
  3. 느낌 표현
  4. 주요 기도문
  5. 설정
- [ ] Marriage Encounter 소개 섹션
- [ ] 카드 호버 효과

### 6.2 설정 페이지 구현
- [ ] 페이지 레이아웃 (`src/pages/Settings.tsx`)
- [ ] 내 정보 관리 섹션
  - 이름, 이메일 표시
  - 비밀번호 변경 버튼
- [ ] 배우자 관리 섹션
  - 배우자 초대 버튼
  - 연결된 배우자 정보 표시
  - 배우자 연결 해제 버튼
- [ ] 계정 관리 섹션
  - 로그아웃 버튼

### 6.3 공통 컴포넌트 구현
- [x] 레이아웃 컴포넌트 (`src/components/Layout.tsx`)
  - 상단 네비게이션 바
  - 로고
  - 사용자 정보
  - 로그아웃 버튼
- [ ] Breadcrumb 컴포넌트 (`src/components/Breadcrumb.tsx`) (선택사항)
- [x] 로딩 스피너 (`src/components/Loading.tsx`)
- [x] 에러 바운더리 (`src/components/ErrorBoundary.tsx`)
- [x] 토스트 알림 컴포넌트 (Ant Design message 사용)
- [x] 확인 다이얼로그 (Ant Design modal 사용)

### 6.4 반응형 디자인 구현
- [x] 모바일 뷰 (320px ~ 768px)
- [x] 태블릿 뷰 (768px ~ 1024px)
- [x] 데스크톱 뷰 (1024px ~ 2560px)
- [x] 미디어 쿼리 설정
- [x] 터치 이벤트 최적화 (모바일)

### 6.5 색상 테마 및 스타일 구현
- [x] CSS 변수 정의 (`src/styles/theme.css`)
  - Primary, Secondary, Background, Text 색상
  - Success, Danger, Warning 색상
  - Border, Shadow 스타일
- [x] 전역 스타일 (`src/styles/global.css`)
- [x] 폰트 설정 (한글 웹폰트)
- [x] 애니메이션 및 전환 효과

### 6.6 접근성 구현
- [x] ARIA 레이블 추가
- [x] 키보드 네비게이션 지원 (Tab, Enter, Esc)
- [x] 포커스 스타일 설정
- [x] 색상 대비 확인 (WCAG 2.1 Level AA)
- [ ] 스크린 리더 테스트 (사용자 테스트 필요)

---

## Phase 7: 보안, 테스트 및 배포 (95% 완료)

### 7.1 Firebase Security Rules 강화 및 문서화 ✅
- [x] Security Rules 문서 작성 (`FIREBASE_SECURITY_RULES.md`)
- [x] 사용자 데이터 보안 규칙
- [x] Topic/TenTen 보안 규칙
- [x] 배우자 연결 보안 규칙
- [x] 필드 검증 규칙

### 7.2 성능 최적화 ✅
- [x] 코드 스플리팅 (React.lazy)
- [x] Lazy loading (모든 페이지 컴포넌트)
- [x] Suspense fallback (전역 Loading 컴포넌트)

### 7.3 전역 에러 핸들링 ✅
- [x] ErrorBoundary 구현
- [x] 전역 에러 캐치
- [x] 사용자 친화적 에러 메시지

### 7.4 문서화 ✅
- [x] README.md 작성
- [x] 설치 및 실행 가이드
- [x] 보안 아키텍처 문서화
- [x] 프로젝트 구조 설명
- [x] DEPLOYMENT_GUIDE.md 작성
- [x] FINAL_REPORT.md 작성

### 7.5 환경 설정 ✅
- [x] .env.example 업데이트
- [x] Firebase 설정 가이드

### 7.6 빌드 테스트 ⚠️
- [x] 프로덕션 빌드 실행
- [ ] TypeScript 에러 수정 (41개 남아있음)
  - TS6137: @types/ import 경고 (24개)
  - TS6133: 사용하지 않는 변수 (10개)
  - TS6192/TS6196: 사용하지 않는 import (4개)
  - 실제 타입 에러 (3개)

## Phase 7: 보안, 테스트 및 배포 (Legacy - 이하 기존 내용)

### 7.1 Firebase Security Rules 작성
- [ ] `users` 노드 보안 규칙
  - 읽기: 본인 또는 연결된 배우자만
  - 쓰기: 본인만
  - 필수 필드 검증
- [ ] `topics` 노드 보안 규칙
  - 읽기/쓰기: 본인만
  - 필수 필드 검증
- [ ] `tentens` 노드 보안 규칙
  - 읽기: 본인 또는 연결된 배우자 (공유 Topic)
  - 쓰기: 본인만
  - 필수 필드 검증
- [ ] `pending_shared_deks` 노드 보안 규칙
  - 읽기/쓰기: 본인만
  - 필수 필드 검증
- [ ] Firebase Console에 규칙 배포

### 7.2 보안 강화
- [ ] XSS 방지 (DOMPurify 사용)
- [ ] CSRF 방지
- [ ] Content Security Policy 설정
- [ ] 민감한 데이터 메모리 삭제 검증
- [ ] 토큰 만료 처리 및 자동 갱신
- [ ] HTTPS 강제 (배포 환경)

### 7.3 성능 최적화
- [ ] 코드 스플리팅 (React.lazy, Suspense)
- [ ] 이미지 최적화
- [ ] Firebase 쿼리 최적화 (인덱스 설정)
- [ ] 메모이제이션 (React.memo, useMemo, useCallback)
- [ ] 번들 크기 최적화
- [ ] 성능 측정 (Lighthouse)
  - 초기 로딩: < 3초
  - 페이지 전환: < 1초
  - 암호화/복호화: < 500ms

### 7.4 에러 핸들링
- [ ] 전역 에러 핸들러 구현
- [ ] 네트워크 에러 처리
- [ ] Firebase 에러 처리 (에러 코드별 메시지)
- [ ] 암호화 실패 처리
- [ ] 사용자 친화적 에러 메시지

### 7.5 테스트 작성
- [ ] 단위 테스트 (Vitest)
  - EncryptionService 테스트
  - AuthService 테스트
  - TopicService 테스트
  - TenTenService 테스트
  - PartnerService 테스트
- [ ] 통합 테스트
  - 회원가입 → 로그인 흐름
  - Topic → TenTen 작성 흐름
  - 배우자 초대 → 조회 흐름
- [ ] E2E 테스트 (Playwright 또는 Cypress)
  - 주요 사용자 시나리오
- [ ] 브라우저 호환성 테스트
  - Chrome, Firefox, Safari, Edge

### 7.6 문서화
- [ ] README.md 작성
  - 프로젝트 소개
  - 설치 및 실행 방법
  - 환경 변수 설정 가이드
  - 기능 설명
- [ ] 개발자 가이드 작성
  - 폴더 구조 설명
  - 컴포넌트 사용 예시
  - API 문서
- [ ] 사용자 매뉴얼 작성 (선택 사항)

### 7.7 배포 준비
- [ ] 프로덕션 빌드 테스트
- [ ] 환경 변수 분리 (개발/프로덕션)
- [ ] Firebase Hosting 설정
- [ ] 도메인 연결 (선택 사항)
- [ ] SSL 인증서 설정
- [ ] 배포 스크립트 작성

### 7.8 배포
- [ ] Firebase Hosting에 배포
  - 또는 Vercel/Netlify 배포
- [ ] 프로덕션 환경 테스트
- [ ] 모니터링 설정 (Firebase Analytics)
- [ ] 에러 추적 설정 (Sentry 등)

---

## Phase 8: 향후 개발 계획 (v1.3+)

### 8.1 우선순위 높음
- [ ] 읽음 확인 기능
  - `isReadByPartner`, `readByPartnerAt` 필드 활용
  - UI에 읽음 상태 표시
- [ ] 알림 시스템
  - 10&10 작성 리마인더
  - 배우자 TenTen 작성 알림
- [ ] 통계 및 분석
  - 월별 작성 횟수
  - 연도별 통계
  - 감정 분석

### 8.2 우선순위 중간
- [ ] 복구 키 백업 시스템
  - 비밀번호 분실 시 복구 방법
  - 안전한 백업 키 생성
- [ ] 오프라인 지원
  - Service Worker 등록
  - 오프라인 데이터 캐싱
  - 동기화 로직
- [ ] PWA 변환
  - manifest.json 작성
  - 아이콘 준비
  - 설치 프롬프트

### 8.3 우선순위 낮음
- [ ] 다크 모드
  - 테마 전환 기능
  - 색상 변수 다크 버전
- [ ] 다국어 지원
  - i18n 설정
  - 한국어, 영어 지원
- [ ] 감정 트렌드 분석
  - 감정 사용 빈도 분석
  - 시각화 (차트)

---

## 체크리스트 요약

### 필수 기능 (MVP)
- [x] 프로젝트 초기화 및 환경 설정
- [ ] Firebase 연동 및 인증 시스템
- [ ] DEK 암호화 시스템
- [ ] 10&10 시스템 (Topic/TenTen CRUD)
- [ ] 배우자 연결 및 공유
- [ ] 느낌 표현
- [ ] 기도문
- [ ] 설정 페이지
- [ ] 홈 페이지 및 네비게이션
- [ ] Firebase Security Rules
- [ ] 배포

### 선택 기능
- [ ] 실시간 자동 저장
- [ ] 읽음 확인
- [ ] 알림 시스템
- [ ] 통계 및 분석
- [ ] 오프라인 지원
- [ ] PWA
- [ ] 다크 모드

---

## 참고 사항

### 개발 순서 권장
1. Phase 0-1: 기본 환경 및 인프라
2. Phase 2: 인증 (회원가입, 로그인)
3. Phase 3: 10&10 기본 기능
4. Phase 6 일부: 홈 페이지 및 네비게이션
5. Phase 4: 배우자 연결
6. Phase 5: 느낌 표현, 기도문
7. Phase 6 나머지: 설정 및 공통 컴포넌트
8. Phase 7: 보안, 테스트, 배포

### 우선순위
- **P0 (Critical)**: Phase 0, 1, 2
- **P1 (High)**: Phase 3, 4, 7.1, 7.7, 7.8
- **P2 (Medium)**: Phase 5, 6, 7.2, 7.3, 7.4
- **P3 (Low)**: Phase 7.5, 7.6, Phase 8

### 예상 소요 시간 (1인 개발 기준)
- Phase 0-1: 3-4일
- Phase 2: 4-5일
- Phase 3: 5-7일
- Phase 4: 4-5일
- Phase 5: 2-3일
- Phase 6: 5-6일
- Phase 7: 5-7일
- **총 예상 시간: 28-37일** (약 6-8주)

---

이 Todo 리스트는 WEBAPP_SPECIFICATION.md의 모든 요구사항을 반영하며, 실제 구현 시 프로젝트 상황에 따라 조정할 수 있습니다.

