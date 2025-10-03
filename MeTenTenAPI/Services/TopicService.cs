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
                .Include(t => t.Diaries)
                .Where(t => t.CreatedByUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                ScheduledDate = t.ScheduledDate,
                IsCompleted = t.IsCompleted,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = t.CreatedByUser.Name,
                DiaryCount = t.Diaries.Count
            });
        }

        public async Task<TopicDto?> GetTopicByIdAsync(int topicId, int userId)
        {
            var topic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .Include(t => t.Diaries)
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return null;

            return new TopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = topic.CreatedAt,
                ScheduledDate = topic.ScheduledDate,
                IsCompleted = topic.IsCompleted,
                CreatedByUserId = topic.CreatedByUserId,
                CreatedByUserName = topic.CreatedByUser.Name,
                DiaryCount = topic.Diaries.Count
            };
        }

        public async Task<TopicDto> CreateTopicAsync(int userId, CreateTopicDto createTopicDto)
        {
            var topic = new Topic
            {
                Title = createTopicDto.Title,
                Description = createTopicDto.Description,
                ScheduledDate = createTopicDto.ScheduledDate,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            var createdTopic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .Include(t => t.Diaries)
                .FirstAsync(t => t.Id == topic.Id);

            return new TopicDto
            {
                Id = createdTopic.Id,
                Title = createdTopic.Title,
                Description = createdTopic.Description,
                CreatedAt = createdTopic.CreatedAt,
                ScheduledDate = createdTopic.ScheduledDate,
                IsCompleted = createdTopic.IsCompleted,
                CreatedByUserId = createdTopic.CreatedByUserId,
                CreatedByUserName = createdTopic.CreatedByUser.Name,
                DiaryCount = createdTopic.Diaries.Count
            };
        }

        public async Task<TopicDto?> UpdateTopicAsync(int topicId, int userId, UpdateTopicDto updateTopicDto)
        {
            var topic = await _context.Topics
                .Include(t => t.CreatedByUser)
                .Include(t => t.Diaries)
                .FirstOrDefaultAsync(t => t.Id == topicId && t.CreatedByUserId == userId);

            if (topic == null) return null;

            if (!string.IsNullOrEmpty(updateTopicDto.Title))
            {
                topic.Title = updateTopicDto.Title;
            }

            if (updateTopicDto.Description != null)
            {
                topic.Description = updateTopicDto.Description;
            }

            if (updateTopicDto.ScheduledDate.HasValue)
            {
                topic.ScheduledDate = updateTopicDto.ScheduledDate.Value;
            }

            if (updateTopicDto.IsCompleted.HasValue)
            {
                topic.IsCompleted = updateTopicDto.IsCompleted.Value;
            }

            await _context.SaveChangesAsync();

            return new TopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = topic.CreatedAt,
                ScheduledDate = topic.ScheduledDate,
                IsCompleted = topic.IsCompleted,
                CreatedByUserId = topic.CreatedByUserId,
                CreatedByUserName = topic.CreatedByUser.Name,
                DiaryCount = topic.Diaries.Count
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
                .Include(t => t.Diaries)
                .Where(t => t.CreatedByUserId == userId && 
                           (t.ScheduledDate == null || t.ScheduledDate.Value.Date == today))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                ScheduledDate = t.ScheduledDate,
                IsCompleted = t.IsCompleted,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = t.CreatedByUser.Name,
                DiaryCount = t.Diaries.Count
            });
        }
    }
}

