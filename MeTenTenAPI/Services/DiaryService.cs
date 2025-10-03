using Microsoft.EntityFrameworkCore;
using MeTenTenAPI.Data;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Models;

namespace MeTenTenAPI.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly ApplicationDbContext _context;

        public DiaryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiaryDto>> GetDiariesAsync(int userId)
        {
            var diaries = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return diaries.Select(d => new DiaryDto
            {
                Id = d.Id,
                Content = d.Content,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                UserId = d.UserId,
                UserName = d.User.Name,
                TopicId = d.TopicId,
                TopicTitle = d.Topic.Title,
                IsReadByPartner = d.IsReadByPartner,
                ReadByPartnerAt = d.ReadByPartnerAt,
                EmotionTag = d.EmotionTag,
                ImportanceLevel = d.ImportanceLevel
            });
        }

        public async Task<IEnumerable<DiaryDto>> GetDiariesByTopicAsync(int topicId, int userId)
        {
            var diaries = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .Where(d => d.TopicId == topicId && d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return diaries.Select(d => new DiaryDto
            {
                Id = d.Id,
                Content = d.Content,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                UserId = d.UserId,
                UserName = d.User.Name,
                TopicId = d.TopicId,
                TopicTitle = d.Topic.Title,
                IsReadByPartner = d.IsReadByPartner,
                ReadByPartnerAt = d.ReadByPartnerAt,
                EmotionTag = d.EmotionTag,
                ImportanceLevel = d.ImportanceLevel
            });
        }

        public async Task<DiaryDto?> GetDiaryByIdAsync(int diaryId, int userId)
        {
            var diary = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserId == userId);

            if (diary == null) return null;

            return new DiaryDto
            {
                Id = diary.Id,
                Content = diary.Content,
                CreatedAt = diary.CreatedAt,
                UpdatedAt = diary.UpdatedAt,
                UserId = diary.UserId,
                UserName = diary.User.Name,
                TopicId = diary.TopicId,
                TopicTitle = diary.Topic.Title,
                IsReadByPartner = diary.IsReadByPartner,
                ReadByPartnerAt = diary.ReadByPartnerAt,
                EmotionTag = diary.EmotionTag,
                ImportanceLevel = diary.ImportanceLevel
            };
        }

        public async Task<DiaryDto> CreateDiaryAsync(int userId, CreateDiaryDto createDiaryDto)
        {
            // 주제가 존재하고 사용자가 접근 가능한지 확인
            var topic = await _context.Topics
                .FirstOrDefaultAsync(t => t.Id == createDiaryDto.TopicId && t.CreatedByUserId == userId);

            if (topic == null)
            {
                throw new ArgumentException("주제를 찾을 수 없습니다.");
            }

            var diary = new Diary
            {
                Content = createDiaryDto.Content,
                UserId = userId,
                TopicId = createDiaryDto.TopicId,
                EmotionTag = createDiaryDto.EmotionTag,
                ImportanceLevel = createDiaryDto.ImportanceLevel,
                CreatedAt = DateTime.UtcNow
            };

            _context.Diaries.Add(diary);
            await _context.SaveChangesAsync();

            var createdDiary = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .FirstAsync(d => d.Id == diary.Id);

            return new DiaryDto
            {
                Id = createdDiary.Id,
                Content = createdDiary.Content,
                CreatedAt = createdDiary.CreatedAt,
                UpdatedAt = createdDiary.UpdatedAt,
                UserId = createdDiary.UserId,
                UserName = createdDiary.User.Name,
                TopicId = createdDiary.TopicId,
                TopicTitle = createdDiary.Topic.Title,
                IsReadByPartner = createdDiary.IsReadByPartner,
                ReadByPartnerAt = createdDiary.ReadByPartnerAt,
                EmotionTag = createdDiary.EmotionTag,
                ImportanceLevel = createdDiary.ImportanceLevel
            };
        }

        public async Task<DiaryDto?> UpdateDiaryAsync(int diaryId, int userId, UpdateDiaryDto updateDiaryDto)
        {
            var diary = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserId == userId);

            if (diary == null) return null;

            if (!string.IsNullOrEmpty(updateDiaryDto.Content))
            {
                diary.Content = updateDiaryDto.Content;
            }

            if (updateDiaryDto.EmotionTag != null)
            {
                diary.EmotionTag = updateDiaryDto.EmotionTag;
            }

            if (updateDiaryDto.ImportanceLevel.HasValue)
            {
                diary.ImportanceLevel = updateDiaryDto.ImportanceLevel.Value;
            }

            diary.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new DiaryDto
            {
                Id = diary.Id,
                Content = diary.Content,
                CreatedAt = diary.CreatedAt,
                UpdatedAt = diary.UpdatedAt,
                UserId = diary.UserId,
                UserName = diary.User.Name,
                TopicId = diary.TopicId,
                TopicTitle = diary.Topic.Title,
                IsReadByPartner = diary.IsReadByPartner,
                ReadByPartnerAt = diary.ReadByPartnerAt,
                EmotionTag = diary.EmotionTag,
                ImportanceLevel = diary.ImportanceLevel
            };
        }

        public async Task<bool> DeleteDiaryAsync(int diaryId, int userId)
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserId == userId);

            if (diary == null) return false;

            _context.Diaries.Remove(diary);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAsReadByPartnerAsync(int diaryId, int userId)
        {
            // 파트너의 일기를 읽음으로 표시
            var user = await _context.Users
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Partner == null) return false;

            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == diaryId && d.UserId == user.Partner.Id);

            if (diary == null) return false;

            diary.IsReadByPartner = true;
            diary.ReadByPartnerAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DiaryDto>> GetUnreadDiariesFromPartnerAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Partner == null) return new List<DiaryDto>();

            var diaries = await _context.Diaries
                .Include(d => d.User)
                .Include(d => d.Topic)
                .Where(d => d.UserId == user.Partner.Id && !d.IsReadByPartner)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return diaries.Select(d => new DiaryDto
            {
                Id = d.Id,
                Content = d.Content,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                UserId = d.UserId,
                UserName = d.User.Name,
                TopicId = d.TopicId,
                TopicTitle = d.Topic.Title,
                IsReadByPartner = d.IsReadByPartner,
                ReadByPartnerAt = d.ReadByPartnerAt,
                EmotionTag = d.EmotionTag,
                ImportanceLevel = d.ImportanceLevel
            });
        }
    }
}

