using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Services;

namespace MeTenTenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.UpdateUserAsync(userId, updateUserDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _userService.ChangePasswordAsync(userId, changePasswordDto);
                
                if (success)
                {
                    return Ok(new { message = "비밀번호가 성공적으로 변경되었습니다." });
                }
                
                return BadRequest(new { message = "현재 비밀번호가 올바르지 않습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("partner")]
        public async Task<IActionResult> GetPartner()
        {
            try
            {
                var userId = GetCurrentUserId();
                var partner = await _userService.GetPartnerAsync(userId);
                
                if (partner == null)
                {
                    return NotFound(new { message = "연결된 파트너가 없습니다." });
                }
                
                return Ok(partner);
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

