using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface ITopicService
    {
        Task<List<Topic>> GetTopicsAsync();
        Task<Topic?> GetTopicByIdAsync(int id);
        Task<Topic> CreateTopicAsync(CreateTopicRequest request);
        Task<Topic> UpdateTopicAsync(int id, UpdateTopicRequest request);
        Task<bool> DeleteTopicAsync(int id);
    }
}

