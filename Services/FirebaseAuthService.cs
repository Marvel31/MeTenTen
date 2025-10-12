using Firebase.Auth;
using Firebase.Auth.Providers;

namespace MeTenTenMaui.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private const string FirebaseApiKey = "AIzaSyAod8lVnNL5Xy_jX7vlnHM8YrhHVHTmiu4";
        private const string TokenKey = "firebase_token";
        private const string UserIdKey = "firebase_user_id";
        private const string UserEmailKey = "firebase_user_email";
        private const string UserNameKey = "firebase_user_name";

        private readonly FirebaseAuthClient _authClient;
        private readonly IEncryptionService _encryptionService;
        private UserCredential? _userCredential;

        public FirebaseAuthService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            
            var config = new FirebaseAuthConfig
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = "metenten-d4a6f.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            _authClient = new FirebaseAuthClient(config);
        }

        public bool IsAuthenticated => _userCredential != null && !string.IsNullOrEmpty(_userCredential.User.Uid);

        public string? CurrentUserId => _userCredential?.User.Uid;

        public string? CurrentUserEmail => _userCredential?.User.Info.Email;

        public string? CurrentUserName
        {
            get
            {
                if (SecureStorage.GetAsync(UserNameKey).Result is string name)
                    return name;
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> SignUpAsync(string email, string password, string name)
        {
            try
            {
                // Firebase에 사용자 등록
                _userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                if (_userCredential != null)
                {
                    // 암호화 서비스 초기화
                    await _encryptionService.InitializeAsync(email, password);
                    
                    // 사용자 정보 저장
                    await SaveUserCredentials(email, name);

                    System.Diagnostics.Debug.WriteLine($"[Auth] 회원가입 성공: {email}");
                    return (true, null);
                }

                return (false, "회원가입에 실패했습니다.");
            }
            catch (FirebaseAuthException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] 회원가입 실패: {ex.Message}");
                return (false, GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] 회원가입 오류: {ex.Message}");
                return (false, "회원가입 중 오류가 발생했습니다.");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> SignInAsync(string email, string password)
        {
            try
            {
                _userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                if (_userCredential != null)
                {
                    // 암호화 서비스 초기화
                    await _encryptionService.InitializeAsync(email, password);
                    
                    await SaveUserCredentials(email, null);

                    System.Diagnostics.Debug.WriteLine($"[Auth] 로그인 성공: {email}");
                    return (true, null);
                }

                return (false, "로그인에 실패했습니다.");
            }
            catch (FirebaseAuthException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] 로그인 실패: {ex.Message}");
                return (false, GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] 로그인 오류: {ex.Message}");
                return (false, "로그인 중 오류가 발생했습니다.");
            }
        }

        public async Task SignOutAsync()
        {
            _userCredential = null;

            // 암호화 키 메모리에서 제거
            _encryptionService.ClearKey();
            
            // 저장된 인증 정보 삭제
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(UserIdKey);
            SecureStorage.Remove(UserEmailKey);
            SecureStorage.Remove(UserNameKey);

            System.Diagnostics.Debug.WriteLine("[Auth] 로그아웃 완료");
            await Task.CompletedTask;
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(TokenKey);
                var email = await SecureStorage.GetAsync(UserEmailKey);

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    // TODO: 토큰으로 자동 로그인 시도
                    // Firebase.Auth 라이브러리의 최신 버전에서는 토큰 갱신을 지원하지만
                    // 현재 버전에서는 이메일/비밀번호 재입력이 필요할 수 있습니다
                    System.Diagnostics.Debug.WriteLine($"[Auth] 저장된 토큰 발견: {email}");
                    return false; // 자동 로그인 미지원 (사용자가 다시 로그인해야 함)
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task SaveUserCredentials(string email, string? name)
        {
            if (_userCredential == null) return;

            try
            {
                var token = await _userCredential.User.GetIdTokenAsync();
                
                await SecureStorage.SetAsync(TokenKey, token);
                await SecureStorage.SetAsync(UserIdKey, _userCredential.User.Uid);
                await SecureStorage.SetAsync(UserEmailKey, email);
                
                if (!string.IsNullOrEmpty(name))
                {
                    await SecureStorage.SetAsync(UserNameKey, name);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Auth] 인증 정보 저장 실패: {ex.Message}");
            }
        }

        private string GetErrorMessage(FirebaseAuthException ex)
        {
            return ex.Reason switch
            {
                AuthErrorReason.EmailExists => "이미 사용 중인 이메일입니다.",
                AuthErrorReason.InvalidEmailAddress => "유효하지 않은 이메일 주소입니다.",
                AuthErrorReason.WeakPassword => "비밀번호가 너무 약합니다. (최소 6자 이상)",
                AuthErrorReason.WrongPassword => "비밀번호가 올바르지 않습니다.",
                AuthErrorReason.UserNotFound => "해당 이메일로 등록된 사용자가 없습니다.",
                AuthErrorReason.TooManyAttemptsTryLater => "너무 많은 시도가 있었습니다. 잠시 후 다시 시도해주세요.",
                _ => $"인증 오류: {ex.Message}"
            };
        }
    }
}

