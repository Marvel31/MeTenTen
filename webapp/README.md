# MeTenTen 웹앱

Marriage Encounter의 10&10 프로그램을 기반으로 한 부부 소통 웹 애플리케이션입니다.

## 🚀 기술 스택

- **Frontend**: React 19 + TypeScript
- **Build Tool**: Vite
- **UI Library**: Ant Design
- **State Management**: Zustand
- **Routing**: React Router v6
- **Backend**: Firebase (Authentication + Realtime Database)
- **Security**: End-to-End Encryption (AES-256-CBC)

## 📦 프로젝트 구조

```
src/
├── components/     # 재사용 가능한 UI 컴포넌트
├── pages/          # 페이지 컴포넌트
├── services/       # 비즈니스 로직 서비스
├── hooks/          # 커스텀 React 훅
├── utils/          # 유틸리티 함수
├── types/          # TypeScript 타입 정의
├── stores/         # Zustand 상태 관리
├── styles/         # 전역 스타일
└── config/         # 설정 파일
```

## 🔧 설치 및 실행

### 사전 요구사항

- Node.js v18 이상
- npm 또는 yarn

### 설치

```bash
npm install
```

### 환경 변수 설정

`.env.example` 파일을 복사하여 `.env` 파일을 생성하고, Firebase 설정을 입력합니다.

```bash
cp .env.example .env
```

### 개발 서버 실행

```bash
npm run dev
```

브라우저에서 [http://localhost:5173](http://localhost:5173)을 엽니다.

### 빌드

```bash
npm run build
```

### 프로덕션 미리보기

```bash
npm run preview
```

## 🛠️ 개발 도구

### 린트

```bash
npm run lint          # 린트 검사
npm run lint:fix      # 린트 자동 수정
```

### 포맷팅

```bash
npm run format        # 코드 포맷팅
npm run format:check  # 포맷팅 검사
```

### 타입 체크

```bash
npm run type-check    # TypeScript 타입 검사
```

## 🔐 보안

이 애플리케이션은 End-to-End 암호화를 사용하여 사용자의 모든 데이터를 보호합니다.

- **DEK 암호화**: PBKDF2 (100,000 iterations, SHA-256)
- **데이터 암호화**: AES-256-CBC
- **개인/공유 암호화**: Personal DEK와 Shared DEK 분리 관리

## 📄 라이선스

이 프로젝트는 비공개 프로젝트입니다.

## 📚 추가 문서

- [기능 명세서](../Doc/WEBAPP_SPECIFICATION.md)
- [구현 Todo 리스트](../Doc/IMPLEMENTATION_TODO.md)
