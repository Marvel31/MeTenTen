# MeTenTen 배포 가이드

이 문서는 MeTenTen 웹앱을 Firebase Hosting에 배포하는 방법을 설명합니다.

## 사전 준비

### 1. Firebase CLI 설치

```bash
npm install -g firebase-tools
```

### 2. Firebase 로그인

```bash
firebase login
```

## Firebase 프로젝트 설정

### 1. Firebase Console 설정

1. [Firebase Console](https://console.firebase.google.com/) 접속
2. 프로젝트 선택 (또는 새 프로젝트 생성)

### 2. Firebase Services 활성화

#### Authentication 설정
1. Firebase Console → Authentication → Sign-in method
2. "이메일/비밀번호" 활성화

#### Realtime Database 설정
1. Firebase Console → Realtime Database → 데이터베이스 만들기
2. "잠금 모드"로 시작 (Security Rules는 나중에 설정)
3. 지역: `asia-southeast1` (싱가포르) 권장

#### Hosting 활성화
1. Firebase Console → Hosting
2. "시작하기" 클릭

### 3. Firebase Security Rules 적용

`FIREBASE_SECURITY_RULES.md` 파일의 내용을 복사하여:

1. Firebase Console → Realtime Database → Rules
2. 규칙 붙여넣기
3. "게시" 버튼 클릭

## 로컬 환경 설정

### 1. 환경 변수 설정

`.env.example` 파일을 복사하여 `.env` 파일 생성:

```bash
cp webapp/.env.example webapp/.env
```

Firebase Console에서 프로젝트 설정 값을 복사하여 `.env` 파일에 입력:

```env
VITE_FIREBASE_API_KEY=your_actual_api_key
VITE_FIREBASE_AUTH_DOMAIN=your_project_id.firebaseapp.com
VITE_FIREBASE_DATABASE_URL=https://your_project_id-default-rtdb.firebaseio.com
VITE_FIREBASE_PROJECT_ID=your_project_id
VITE_FIREBASE_STORAGE_BUCKET=your_project_id.appspot.com
VITE_FIREBASE_MESSAGING_SENDER_ID=your_messaging_sender_id
VITE_FIREBASE_APP_ID=your_app_id
```

### 2. 의존성 설치

```bash
cd webapp
npm install
```

### 3. 로컬 테스트

```bash
npm run dev
```

브라우저에서 `http://localhost:5173` 접속하여 테스트

## Firebase Hosting 초기화

프로젝트 루트 디렉토리에서:

```bash
firebase init hosting
```

질문에 대한 답변:

1. **Use an existing project** 선택
2. 프로젝트 선택
3. **What do you want to use as your public directory?** → `webapp/dist`
4. **Configure as a single-page app (rewrite all urls to /index.html)?** → **Yes**
5. **Set up automatic builds and deploys with GitHub?** → **No** (또는 필요시 Yes)
6. **File webapp/dist/index.html already exists. Overwrite?** → **No**

## 프로덕션 빌드

```bash
cd webapp
npm run build
```

빌드 결과물은 `webapp/dist` 디렉토리에 생성됩니다.

### 빌드 미리보기

```bash
npm run preview
```

`http://localhost:4173`에서 프로덕션 빌드 미리보기

## 배포

### 첫 번째 배포

프로젝트 루트 디렉토리에서:

```bash
firebase deploy --only hosting
```

배포가 완료되면 Hosting URL이 표시됩니다:
- `https://your-project-id.web.app`
- `https://your-project-id.firebaseapp.com`

### 배포 미리보기 (선택사항)

```bash
firebase hosting:channel:deploy preview
```

임시 URL에서 테스트 가능합니다.

## 배포 후 확인 사항

### 1. 기능 테스트

- [ ] 회원가입
- [ ] 로그인
- [ ] 10&10 주제 생성
- [ ] 10&10 작성
- [ ] 배우자 초대
- [ ] 배우자 10&10 조회
- [ ] 느낌 표현 조회
- [ ] 기도문 조회
- [ ] 모바일 반응형 테스트

### 2. 성능 확인

Chrome DevTools의 Lighthouse를 사용하여 성능 측정:

```bash
- Performance: > 90
- Accessibility: > 90
- Best Practices: > 90
- SEO: > 90
```

### 3. 브라우저 호환성 테스트

- [ ] Chrome (최신)
- [ ] Firefox (최신)
- [ ] Safari (최신)
- [ ] Edge (최신)
- [ ] Mobile Safari (iOS)
- [ ] Chrome Mobile (Android)

## 커스텀 도메인 설정 (선택사항)

### 1. 도메인 추가

1. Firebase Console → Hosting
2. "맞춤 도메인 추가" 클릭
3. 도메인 입력 (예: `metenten.com`)

### 2. DNS 설정

Firebase가 제공하는 DNS 레코드를 도메인 제공업체에 추가:

- A 레코드
- AAAA 레코드 (IPv6)
- TXT 레코드 (소유권 확인)

### 3. SSL 인증서

Firebase가 자동으로 SSL 인증서를 발급하고 관리합니다 (Let's Encrypt).

## 환경별 배포 전략

### 개발 환경

```bash
firebase hosting:channel:deploy dev
```

### 스테이징 환경

```bash
firebase hosting:channel:deploy staging --expires 30d
```

### 프로덕션 환경

```bash
firebase deploy --only hosting
```

## CI/CD 설정 (선택사항)

### GitHub Actions

`.github/workflows/firebase-hosting.yml` 파일 생성:

```yaml
name: Deploy to Firebase Hosting

on:
  push:
    branches:
      - main

jobs:
  build_and_deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        run: cd webapp && npm ci
      
      - name: Build
        run: cd webapp && npm run build
        env:
          VITE_FIREBASE_API_KEY: ${{ secrets.VITE_FIREBASE_API_KEY }}
          VITE_FIREBASE_AUTH_DOMAIN: ${{ secrets.VITE_FIREBASE_AUTH_DOMAIN }}
          VITE_FIREBASE_DATABASE_URL: ${{ secrets.VITE_FIREBASE_DATABASE_URL }}
          VITE_FIREBASE_PROJECT_ID: ${{ secrets.VITE_FIREBASE_PROJECT_ID }}
          VITE_FIREBASE_STORAGE_BUCKET: ${{ secrets.VITE_FIREBASE_STORAGE_BUCKET }}
          VITE_FIREBASE_MESSAGING_SENDER_ID: ${{ secrets.VITE_FIREBASE_MESSAGING_SENDER_ID }}
          VITE_FIREBASE_APP_ID: ${{ secrets.VITE_FIREBASE_APP_ID }}
      
      - name: Deploy to Firebase
        uses: FirebaseExtended/action-hosting-deploy@v0
        with:
          repoToken: '${{ secrets.GITHUB_TOKEN }}'
          firebaseServiceAccount: '${{ secrets.FIREBASE_SERVICE_ACCOUNT }}'
          channelId: live
          projectId: your-project-id
```

GitHub Secrets 설정:
- `VITE_FIREBASE_*`: Firebase 설정 값들
- `FIREBASE_SERVICE_ACCOUNT`: Firebase 서비스 계정 JSON

## 롤백

이전 버전으로 롤백:

```bash
firebase hosting:rollback
```

## 모니터링

### Firebase Console

- Hosting → Dashboard에서 트래픽 및 성능 확인
- Realtime Database → Usage에서 데이터 사용량 확인
- Authentication → Users에서 사용자 통계 확인

### Google Analytics (선택사항)

Firebase Console → Analytics에서 Google Analytics 연동 가능

## 비용 관리

### 무료 할당량 (Spark Plan)

- **Hosting**: 10 GB 저장소, 360 MB/day 전송
- **Realtime Database**: 1 GB 저장소, 10 GB/month 다운로드
- **Authentication**: 무제한

### Blaze Plan (종량제)

더 많은 트래픽이 필요한 경우 Blaze Plan으로 업그레이드:
- Firebase Console → 요금제 → 업그레이드

## 문제 해결

### 배포 실패

```bash
# 캐시 삭제
firebase hosting:channel:delete preview

# 다시 배포
firebase deploy --only hosting --force
```

### CORS 에러

`firebase.json`에 CORS 헤더 추가:

```json
{
  "hosting": {
    "public": "webapp/dist",
    "ignore": ["firebase.json", "**/.*", "**/node_modules/**"],
    "rewrites": [
      {
        "source": "**",
        "destination": "/index.html"
      }
    ],
    "headers": [
      {
        "source": "**",
        "headers": [
          {
            "key": "Access-Control-Allow-Origin",
            "value": "*"
          }
        ]
      }
    ]
  }
}
```

### 환경 변수 누락

빌드 시 환경 변수가 제대로 로드되는지 확인:

```bash
# 환경 변수 출력 (테스트용)
cd webapp
node -e "console.log(process.env)"
```

## 보안 체크리스트

배포 전 확인:

- [ ] Firebase Security Rules 적용 완료
- [ ] `.env` 파일이 `.gitignore`에 포함됨
- [ ] API 키가 GitHub에 노출되지 않음
- [ ] HTTPS 강제 (Firebase Hosting 기본)
- [ ] CSP (Content Security Policy) 설정 검토

## 성능 최적화

### 1. CDN 캐싱

Firebase Hosting은 자동으로 전 세계 CDN을 통해 콘텐츠를 제공합니다.

### 2. 브라우저 캐싱

`firebase.json`에 캐시 설정:

```json
{
  "hosting": {
    "headers": [
      {
        "source": "**/*.@(js|css)",
        "headers": [
          {
            "key": "Cache-Control",
            "value": "max-age=31536000, immutable"
          }
        ]
      }
    ]
  }
}
```

### 3. 이미지 최적화

WebP 형식 사용 및 lazy loading 적용

## 지원

문제가 발생하면:

1. Firebase Console → 지원 → 문서 검색
2. [Firebase 공식 문서](https://firebase.google.com/docs)
3. [StackOverflow](https://stackoverflow.com/questions/tagged/firebase)

---

**배포를 축하합니다! 🎉**





