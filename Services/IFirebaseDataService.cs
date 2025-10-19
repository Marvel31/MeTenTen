using MeTenTenMaui.Models;
using MeTenTenMaui.Models.Firebase;

namespace MeTenTenMaui.Services
{
    public interface IFirebaseDataService
    {
        // User DEK Management
        Task SaveUserDEKAsync(string userId, string email, string displayName, string encryptedDEK);
        Task<string?> GetUserDEKAsync(string userId);
        
        // Partner Management
        Task<FirebaseUser?> GetUserByEmailAsync(string email);
        Task<(FirebaseUser? user, string? userId)> GetUserWithIdByEmailAsync(string email);
        Task<FirebaseUser?> GetUserAsync(string userId);
        Task UpdatePartnerInfoAsync(string userId, PartnerInfo partnerInfo);
        Task RemovePartnerInfoAsync(string userId);

        // Topic CRUD
        Task<List<Topic>> GetTopicsAsync(string userId);
        Task<List<Topic>> GetAllTopicsAsync(string userId);
        Task<Topic?> GetTopicByIdAsync(string userId, string topicId);
        Task<Topic> CreateTopicAsync(string userId, CreateTopicRequest request);
        Task<Topic> UpdateTopicAsync(string userId, string topicId, UpdateTopicRequest request);
        Task<bool> DeleteTopicAsync(string userId, string topicId);

        // TenTen CRUD
        Task<List<TenTen>> GetTenTensAsync(string userId);
        Task<TenTen?> GetTenTenByIdAsync(string userId, string tenTenId);
        Task<List<TenTen>> GetTenTensByTopicAsync(string userId, string topicFirebaseKey);
        Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request);
        Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request, string encryptionType);
        Task<TenTen> UpdateTenTenAsync(string userId, string tenTenId, UpdateTenTenRequest request);
        Task<bool> DeleteTenTenAsync(string userId, string tenTenId);
        Task<bool> DeleteTenTensByTopicAsync(string userId, string topicFirebaseKey);
        
        // Pending Shared DEK Management
        Task SavePendingSharedDEKAsync(string userId, byte[] sharedDek, string inviterUserId);
        Task<byte[]?> GetPendingSharedDEKAsync(string userId);
        Task DeletePendingSharedDEKAsync(string userId);
        Task UpdatePartnerSharedDEKAsync(string userId, string encryptedSharedDEK);

        // 실시간 동기화를 위한 리스너 메서드
        Task<IDisposable> ObservePartnerTopicsAsync(string partnerUserId, Action<List<Topic>> onTopicsChanged);
        Task<IDisposable> ObservePartnerTenTensAsync(string partnerUserId, string topicFirebaseKey, Action<List<TenTen>> onTenTensChanged);
    }
}

