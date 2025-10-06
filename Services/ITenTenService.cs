using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface ITenTenService
    {
        Task<List<TenTen>> GetTenTensAsync();
        Task<List<TenTen>> GetAllTenTensAsync();
        Task<List<TenTen>> GetTenTensByTopicAsync(int topicId);
        Task<TenTen?> GetTenTenByIdAsync(int id);
        Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request);
        Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request);
        Task<TenTen> UpdateTenTenAsync(TenTen tenTen);
        Task<bool> DeleteTenTenAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
    }
}

