# Firebase Realtime Database Security Rules

이 문서는 MeTenTen 앱의 Firebase Realtime Database Security Rules를 설명합니다.

## 규칙 개요

MeTenTen 앱은 다음과 같은 보안 요구사항을 가지고 있습니다:

1. **인증된 사용자만 데이터 접근 가능**
2. **사용자는 자신의 데이터만 읽고 쓸 수 있음**
3. **배우자로 연결된 경우, 상대방의 공유 TenTen 데이터 읽기 가능**
4. **데이터 구조 및 필수 필드 검증**

## Security Rules 코드

아래 규칙을 Firebase Console → Realtime Database → Rules에 복사하여 적용하세요.

```json
{
  "rules": {
    "users": {
      ".indexOn": ["email"],
      "$userId": {
        ".read": "auth != null && (auth.uid === $userId || (root.child('users').child(auth.uid).child('partner').child('partnerId').val() === $userId))",
        ".write": "auth != null && auth.uid === $userId",
        "email": {
          ".validate": "newData.isString() && newData.val().matches(/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}$/i)"
        },
        "displayName": {
          ".validate": "newData.isString() && newData.val().length > 0 && newData.val().length <= 100"
        },
        "encryptedDEK": {
          ".validate": "newData.isString() && newData.val().length > 0"
        },
        "partner": {
          "partnerId": {
            ".validate": "newData.isString()"
          },
          "partnerEmail": {
            ".validate": "newData.isString()"
          },
          "partnerName": {
            ".validate": "newData.isString()"
          },
          "encryptedSharedDEK": {
            ".validate": "newData.isString()"
          },
          "connectedAt": {
            ".validate": "newData.isString()"
          }
        },
        "createdAt": {
          ".validate": "newData.isString()"
        },
        "updatedAt": {
          ".validate": "newData.isString()"
        }
      }
    },
    "topics": {
      "$userId": {
        ".read": "auth != null && auth.uid === $userId",
        ".write": "auth != null && auth.uid === $userId",
        "$topicId": {
          ".validate": "newData.hasChildren(['title', 'createdAt'])",
          "title": {
            ".validate": "newData.isString() && newData.val().length > 0 && newData.val().length <= 200"
          },
          "createdAt": {
            ".validate": "newData.isString()"
          },
          "updatedAt": {
            ".validate": "newData.isString()"
          }
        }
      }
    },
    "tentens": {
      "$userId": {
        ".read": "auth != null && (auth.uid === $userId || (root.child('users').child(auth.uid).child('partner').child('partnerId').val() === $userId))",
        ".write": "auth != null && auth.uid === $userId",
        "$tentenId": {
          ".validate": "newData.hasChildren(['topicId', 'encryptedContent', 'encryptionType', 'createdAt'])",
          "topicId": {
            ".validate": "newData.isString()"
          },
          "encryptedContent": {
            ".validate": "newData.isString() && newData.val().length > 0"
          },
          "encryptionType": {
            ".validate": "newData.isString() && (newData.val() === 'personal' || newData.val() === 'shared')"
          },
          "isRead": {
            ".validate": "newData.isBoolean()"
          },
          "createdAt": {
            ".validate": "newData.isString()"
          },
          "updatedAt": {
            ".validate": "newData.isString()"
          }
        }
      }
    },
    "pending_shared_deks": {
      "$userId": {
        ".read": "auth != null && auth.uid === $userId",
        ".write": "auth != null",
        "encryptedSharedDEK": {
          ".validate": "newData.isString()"
        },
        "fromUserId": {
          ".validate": "newData.isString()"
        },
        "fromUserEmail": {
          ".validate": "newData.isString()"
        },
        "createdAt": {
          ".validate": "newData.isString()"
        }
      }
    }
  }
}
```

## 규칙 설명

### users 노드
- **읽기**: 본인 데이터 또는 연결된 배우자의 데이터만 읽기 가능
- **쓰기**: 본인 데이터만 쓰기 가능
- **이메일 인덱싱**: 이메일로 사용자 검색 가능 (`.indexOn: ["email"]`)
- **필드 검증**:
  - `email`: 이메일 형식 검증
  - `displayName`: 1-100자 문자열
  - `encryptedDEK`: 필수 문자열
  - `partner`: 배우자 정보 구조 검증

### topics 노드
- **읽기/쓰기**: 본인의 Topic만 접근 가능
- **필드 검증**:
  - `title`: 1-200자 필수 문자열
  - `createdAt`, `updatedAt`: ISO 날짜 문자열

### tentens 노드
- **읽기**: 본인 데이터 또는 연결된 배우자의 데이터 읽기 가능 (공유 TenTen 조회용)
- **쓰기**: 본인 데이터만 쓰기 가능
- **필드 검증**:
  - `topicId`: 필수 문자열
  - `encryptedContent`: 필수 문자열 (암호화된 내용)
  - `encryptionType`: 'personal' 또는 'shared'만 허용
  - `isRead`: 불리언

### pending_shared_deks 노드
- **읽기**: 본인의 pending DEK만 읽기 가능
- **쓰기**: 인증된 모든 사용자 (배우자 초대를 위해)
- **필드 검증**:
  - `encryptedSharedDEK`: 필수 문자열
  - `fromUserId`, `fromUserEmail`: 초대자 정보

## 적용 방법

1. Firebase Console에 로그인
2. 프로젝트 선택
3. Realtime Database → Rules 탭으로 이동
4. 위의 JSON 규칙 복사
5. 붙여넣기 후 "게시" 버튼 클릭

## 보안 주의사항

1. **암호화된 데이터**: 모든 민감한 데이터는 클라이언트에서 암호화되어 저장됩니다.
2. **DEK 보안**: DEK는 사용자 비밀번호로 암호화되어 저장되며, 메모리에만 평문으로 존재합니다.
3. **공유 DEK**: 배우자 간 공유 DEK는 각자의 개인 DEK로 암호화되어 저장됩니다.
4. **읽기 권한**: 배우자는 상대방의 '공유' 타입 TenTen만 읽을 수 있습니다.

## 테스트

Rules Playground를 사용하여 다음 시나리오를 테스트하세요:

1. ✅ 인증된 사용자가 자신의 데이터 읽기/쓰기
2. ❌ 인증된 사용자가 다른 사용자의 데이터 읽기/쓰기
3. ✅ 배우자로 연결된 사용자가 상대방의 TenTen 읽기
4. ❌ 배우자가 아닌 사용자가 다른 사용자의 TenTen 읽기
5. ❌ 잘못된 데이터 형식으로 쓰기 시도

## 문제 해결

### "Permission denied" 에러
- 사용자가 인증되었는지 확인
- 접근하려는 데이터의 소유자가 맞는지 확인
- 배우자 연결이 제대로 되어 있는지 확인

### "Index not defined" 경고
- `.indexOn: ["email"]` 규칙이 올바르게 적용되었는지 확인
- Firebase Console에서 인덱스 생성 확인





