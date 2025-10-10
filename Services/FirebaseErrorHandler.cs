using System;

namespace MeTenTenMaui.Services
{
    /// <summary>
    /// Firebase 오류 처리 유틸리티
    /// </summary>
    public static class FirebaseErrorHandler
    {
        /// <summary>
        /// Firebase 오류를 사용자 친화적인 메시지로 변환
        /// </summary>
        public static string GetUserFriendlyMessage(Exception ex)
        {
            var message = ex.Message.ToLower();

            // 인증 오류
            if (message.Contains("permission") || message.Contains("denied") || message.Contains("unauthorized"))
            {
                return "권한이 없습니다. 다시 로그인해주세요.";
            }

            // 네트워크 오류
            if (message.Contains("network") || message.Contains("connection") || message.Contains("timeout"))
            {
                return "네트워크 연결을 확인해주세요.";
            }

            // 인증서 오류
            if (message.Contains("email") && message.Contains("already"))
            {
                return "이미 사용 중인 이메일입니다.";
            }

            // 비밀번호 오류
            if (message.Contains("password") && message.Contains("weak"))
            {
                return "비밀번호는 6자 이상이어야 합니다.";
            }

            // 잘못된 로그인 정보
            if (message.Contains("invalid") || message.Contains("wrong"))
            {
                return "이메일 또는 비밀번호가 올바르지 않습니다.";
            }

            // 데이터 유효성 오류
            if (message.Contains("validation") || message.Contains("invalid data"))
            {
                return "입력한 데이터가 올바르지 않습니다.";
            }

            // 기본 오류 메시지
            return "오류가 발생했습니다. 잠시 후 다시 시도해주세요.";
        }

        /// <summary>
        /// Firebase 오류를 로깅
        /// </summary>
        public static void LogError(string context, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Firebase Error] {context}: {ex.Message}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase Error] Inner: {ex.InnerException.Message}");
            }
        }
    }
}

