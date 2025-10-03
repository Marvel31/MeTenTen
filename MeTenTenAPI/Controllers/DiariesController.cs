using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Services;

namespace MeTenTenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // 개발 중 인증 비활성화
    public class DiariesController : ControllerBase
    {
        private readonly IDiaryService _diaryService;

        public DiariesController(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiaries()
        {
            try
            {
                var userId = GetCurrentUserId();
                var diaries = await _diaryService.GetDiariesAsync(userId);
                return Ok(diaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("topic/{topicId}")]
        public async Task<IActionResult> GetDiariesByTopic(int topicId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var diaries = await _diaryService.GetDiariesByTopicAsync(topicId, userId);
                return Ok(diaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadDiariesFromPartner()
        {
            try
            {
                var userId = GetCurrentUserId();
                var diaries = await _diaryService.GetUnreadDiariesFromPartnerAsync(userId);
                return Ok(diaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiary(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var diary = await _diaryService.GetDiaryByIdAsync(id, userId);
                
                if (diary == null)
                {
                    return NotFound(new { message = "일기를 찾을 수 없습니다." });
                }
                
                return Ok(diary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiary([FromBody] CreateDiaryDto createDiaryDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var diary = await _diaryService.CreateDiaryAsync(userId, createDiaryDto);
                return CreatedAtAction(nameof(GetDiary), new { id = diary.Id }, diary);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiary(int id, [FromBody] UpdateDiaryDto updateDiaryDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var diary = await _diaryService.UpdateDiaryAsync(id, userId, updateDiaryDto);
                
                if (diary == null)
                {
                    return NotFound(new { message = "일기를 찾을 수 없습니다." });
                }
                
                return Ok(diary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiary(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _diaryService.DeleteDiaryAsync(id, userId);
                
                if (!success)
                {
                    return NotFound(new { message = "일기를 찾을 수 없습니다." });
                }
                
                return Ok(new { message = "일기가 성공적으로 삭제되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _diaryService.MarkAsReadByPartnerAsync(id, userId);
                
                if (!success)
                {
                    return NotFound(new { message = "일기를 찾을 수 없습니다." });
                }
                
                return Ok(new { message = "일기를 읽음으로 표시했습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            // 개발 중에는 더미 사용자 ID 사용
            return 1;
            
            // 실제 인증이 필요한 경우 아래 코드 사용
            /*
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("유효하지 않은 사용자입니다.");
            }
            return userId;
            */
        }
    }
}

