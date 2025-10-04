# Azure App Service 배포 가이드

## 1. Azure 계정 생성
- https://portal.azure.com 에서 무료 계정 생성
- 신용카드 없이도 무료 티어 사용 가능

## 2. App Service 생성
1. Azure Portal에서 "App Services" 클릭
2. "Create" 버튼 클릭
3. 설정:
   - Resource Group: 새로 생성
   - Name: metenten-app (고유한 이름)
   - Runtime: .NET 8
   - Operating System: Windows
   - Region: Korea Central

## 3. 데이터베이스 설정
1. "SQL Database" 생성
2. Server: 새로 생성
3. Database name: MeTenTenDB
4. Connection string을 App Service 설정에 추가

## 4. 배포
1. Visual Studio에서 "Publish" 클릭
2. "Azure App Service" 선택
3. 생성한 App Service 선택
4. "Publish" 클릭

## 5. 도메인 설정
- 기본 URL: https://metenten-app.azurewebsites.net
- 사용자 정의 도메인도 연결 가능
