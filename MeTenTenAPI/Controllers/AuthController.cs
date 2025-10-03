using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Services;

namespace MeTenTenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var token = await _authService.RegisterAsync(createUserDto);
                return Ok(new { token });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("link-partner")]
        public async Task<IActionResult> LinkPartner([FromBody] LinkPartnerDto linkPartnerDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _authService.LinkPartnerAsync(userId, linkPartnerDto);
                
                if (success)
                {
                    return Ok(new { message = "파트너가 성공적으로 연결되었습니다." });
                }
                
                return BadRequest(new { message = "파트너 연결에 실패했습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _authService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    return NotFound(new { message = "사용자를 찾을 수 없습니다." });
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("유효하지 않은 사용자입니다.");
            }
            return userId;
        }
    }
}

