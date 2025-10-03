using MeTenTenAPI.DTOs;

namespace MeTenTenAPI.Services
{
    public interface IDiaryService
    {
        Task<IEnumerable<DiaryDto>> GetDiariesAsync(int userId);
        Task<IEnumerable<DiaryDto>> GetDiariesByTopicAsync(int topicId, int userId);
        Task<DiaryDto?> GetDiaryByIdAsync(int diaryId, int userId);
        Task<DiaryDto> CreateDiaryAsync(int userId, CreateDiaryDto createDiaryDto);
        Task<DiaryDto?> UpdateDiaryAsync(int diaryId, int userId, UpdateDiaryDto updateDiaryDto);
        Task<bool> DeleteDiaryAsync(int diaryId, int userId);
        Task<bool> MarkAsReadByPartnerAsync(int diaryId, int userId);
        Task<IEnumerable<DiaryDto>> GetUnreadDiariesFromPartnerAsync(int userId);
    }
}

