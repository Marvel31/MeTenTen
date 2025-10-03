using MeTenTenBlazor.Models;

namespace MeTenTenBlazor.Services
{
    public interface ITenTenService
    {
        Task<List<TenTen>> GetTenTensAsync();
        Task<List<TenTen>> GetTenTensByTopicAsync(int topicId);
        Task<TenTen?> GetTenTenByIdAsync(int id);
        Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request);
        Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request);
        Task<bool> DeleteTenTenAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
    }
}
