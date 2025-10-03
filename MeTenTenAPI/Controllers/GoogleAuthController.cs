using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeTenTenAPI.Services;
using MeTenTenAPI.DTOs;

namespace MeTenTenAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public GoogleAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback")
            };
            
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Google 인증에 실패했습니다." });
            }

            var claims = result.Principal?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                return BadRequest(new { message = "Google에서 필요한 정보를 가져올 수 없습니다." });
            }

            try
            {
                // 기존 사용자 확인 또는 새 사용자 생성
                var existingUser = await _authService.GetUserByEmailAsync(email);
                
                if (existingUser == null)
                {
                    // 새 사용자 생성
                    var createUserDto = new CreateUserDto
                    {
                        Name = name,
                        Email = email,
                        Password = GenerateRandomPassword() // Google 사용자는 임시 비밀번호 생성
                    };
                    
                    var token = await _authService.RegisterAsync(createUserDto);
                    return Redirect($"/login-success?token={token}");
                }
                else
                {
                    // 기존 사용자 로그인
                    var loginDto = new LoginDto
                    {
                        Email = email,
                        Password = "google-oauth-user" // Google 사용자는 특별한 비밀번호 사용
                    };
                    
                    var token = await _authService.LoginAsync(loginDto);
                    return Redirect($"/login-success?token={token}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"인증 처리 중 오류가 발생했습니다: {ex.Message}" });
            }
        }

        private string GenerateRandomPassword()
        {
            // Google OAuth 사용자를 위한 임시 비밀번호 생성
            return "GoogleOAuth_" + Guid.NewGuid().ToString("N")[..16];
        }
    }
}
