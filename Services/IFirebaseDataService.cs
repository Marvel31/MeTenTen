using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface IFirebaseDataService
    {
        // User DEK Management
        Task SaveUserDEKAsync(string userId, string email, string displayName, string encryptedDEK);
        Task<string?> GetUserDEKAsync(string userId);

        // Topic CRUD
        Task<List<Topic>> GetTopicsAsync(string userId);
        Task<Topic?> GetTopicByIdAsync(string userId, string topicId);
        Task<Topic> CreateTopicAsync(string userId, CreateTopicRequest request);
        Task<Topic> UpdateTopicAsync(string userId, string topicId, UpdateTopicRequest request);
        Task<bool> DeleteTopicAsync(string userId, string topicId);

        // TenTen CRUD
        Task<List<TenTen>> GetTenTensAsync(string userId);
        Task<TenTen?> GetTenTenByIdAsync(string userId, string tenTenId);
        Task<List<TenTen>> GetTenTensByTopicAsync(string userId, int topicId);
        Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request);
        Task<TenTen> UpdateTenTenAsync(string userId, string tenTenId, UpdateTenTenRequest request);
        Task<bool> DeleteTenTenAsync(string userId, string tenTenId);
        Task<bool> DeleteTenTensByTopicAsync(string userId, int topicId);
    }
}

