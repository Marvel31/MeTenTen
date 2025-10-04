# Railway 배포 가이드

## 1. Railway 계정 생성
- https://railway.app 에서 GitHub 계정으로 로그인

## 2. 프로젝트 연결
1. "New Project" 클릭
2. "Deploy from GitHub repo" 선택
3. MeTenTen 저장소 선택

## 3. 환경 변수 설정
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=your_database_url
```

## 4. 데이터베이스 추가
1. "Add Service" → "Database" → "PostgreSQL"
2. 자동으로 연결됨

## 5. 배포
- GitHub에 푸시하면 자동 배포
- URL: https://metenten-production.up.railway.app
