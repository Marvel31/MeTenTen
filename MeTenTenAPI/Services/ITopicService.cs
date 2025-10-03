using MeTenTenAPI.DTOs;

namespace MeTenTenAPI.Services
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicDto>> GetTopicsAsync(int userId);
        Task<TopicDto?> GetTopicByIdAsync(int topicId, int userId);
        Task<TopicDto> CreateTopicAsync(int userId, CreateTopicDto createTopicDto);
        Task<TopicDto?> UpdateTopicAsync(int topicId, int userId, UpdateTopicDto updateTopicDto);
        Task<bool> DeleteTopicAsync(int topicId, int userId);
        Task<IEnumerable<TopicDto>> GetTodaysTopicsAsync(int userId);
    }
}

