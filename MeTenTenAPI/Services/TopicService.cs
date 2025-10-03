using Microsoft.EntityFrameworkCore;
using MeTenTenAPI.Data;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Models;

namespace MeTenTenAPI.Services
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _context;

        public TopicService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int userId)
        {
            var topics = await _context.Topics
                .Include(t => t.CreatedByUser)
                .Where(t => t.CreatedByUserId == userId)
                .OrderByDescending(t => t.TopicDate)
                .ToListAsync();

            return topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Subject = t.Subject,
                TopicDate = t.TopicDate,
                CreatedAt = t.CreatedAt,
                IsActive = t.IsActive,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = t.CreatedByUser.Name
            });
        }

        public async Task<TopicDto?> GetTopicByIdAsync(int topicId, int userId)
        {
            var topic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return null;

            return new TopicDto
            {
                Id = topic.Id,
                Subject = topic.Subject,
                TopicDate = topic.TopicDate,
                CreatedAt = topic.CreatedAt,
                IsActive = topic.IsActive,
                CreatedByUserId = topic.CreatedByUserId,
                CreatedByUserName = topic.CreatedByUser.Name
            };
        }

        public async Task<TopicDto> CreateTopicAsync(int userId, CreateTopicDto createTopicDto)
        {
            var topic = new Topic
            {
                Subject = createTopicDto.Subject,
                TopicDate = createTopicDto.TopicDate,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            var createdTopic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .FirstAsync(t => t.Id == topic.Id);

            return new TopicDto
            {
                Id = createdTopic.Id,
                Subject = createdTopic.Subject,
                TopicDate = createdTopic.TopicDate,
                CreatedAt = createdTopic.CreatedAt,
                IsActive = createdTopic.IsActive,
                CreatedByUserId = createdTopic.CreatedByUserId,
                CreatedByUserName = createdTopic.CreatedByUser.Name
            };
        }

        public async Task<TopicDto?> UpdateTopicAsync(int topicId, int userId, UpdateTopicDto updateTopicDto)
        {
            var topic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return null;

            topic.Subject = updateTopicDto.Subject;
            topic.TopicDate = updateTopicDto.TopicDate;
            topic.IsActive = updateTopicDto.IsActive;

            await _context.SaveChangesAsync();

            return new TopicDto
            {
                Id = topic.Id,
                Subject = topic.Subject,
                TopicDate = topic.TopicDate,
                CreatedAt = topic.CreatedAt,
                IsActive = topic.IsActive,
                CreatedByUserId = topic.CreatedByUserId,
                CreatedByUserName = topic.CreatedByUser.Name
            };
        }

        public async Task<bool> DeleteTopicAsync(int topicId, int userId)
        {
            var topic = await _context.Topics
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return false;

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TopicDto>> GetTodaysTopicsAsync(int userId)
        {
            var today = DateTime.Today;
            var topics = await _context.Topics
                .Include(t => t.CreatedByUser)
                .Where(t => t.CreatedByUserId == userId && t.TopicDate.Date == today)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Subject = t.Subject,
                TopicDate = t.TopicDate,
                CreatedAt = t.CreatedAt,
                IsActive = t.IsActive,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = t.CreatedByUser.Name
            });
        }

        public async Task<bool> ToggleTopicStatusAsync(int topicId, int userId)
        {
            var topic = await _context.Topics
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return false;

            topic.IsActive = !topic.IsActive;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}