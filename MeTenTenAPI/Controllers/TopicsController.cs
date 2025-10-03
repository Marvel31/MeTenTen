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
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopics()
        {
            try
            {
                var userId = GetCurrentUserId();
                var topics = await _topicService.GetTopicsAsync(userId);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysTopics()
        {
            try
            {
                var userId = GetCurrentUserId();
                var topics = await _topicService.GetTodaysTopicsAsync(userId);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var topic = await _topicService.GetTopicByIdAsync(id, userId);
                
                if (topic == null)
                {
                    return NotFound(new { message = "주제를 찾을 수 없습니다." });
                }
                
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto createTopicDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var topic = await _topicService.CreateTopicAsync(userId, createTopicDto);
                return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateTopicDto updateTopicDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var topic = await _topicService.UpdateTopicAsync(id, userId, updateTopicDto);
                
                if (topic == null)
                {
                    return NotFound(new { message = "주제를 찾을 수 없습니다." });
                }
                
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _topicService.DeleteTopicAsync(id, userId);
                
                if (!success)
                {
                    return NotFound(new { message = "주제를 찾을 수 없습니다." });
                }
                
                return Ok(new { message = "주제가 성공적으로 삭제되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleTopicStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _topicService.ToggleTopicStatusAsync(id, userId);
                
                if (!success)
                {
                    return NotFound(new { message = "주제를 찾을 수 없습니다." });
                }
                
                return Ok(new { message = "주제 상태가 성공적으로 변경되었습니다." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            // 개발 중에는 더미 사용자 ID 사용
            // 데이터베이스에 사용자가 없으면 생성
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

