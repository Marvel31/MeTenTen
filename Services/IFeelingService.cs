using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface IFeelingService
    {
        Task<List<Feeling>> GetAllFeelingsAsync();
        Task<Feeling?> GetFeelingByIdAsync(int id);
        Task<Feeling> CreateFeelingAsync(Feeling feeling);
        Task<Feeling> UpdateFeelingAsync(Feeling feeling);
        Task DeleteFeelingAsync(int id);
        Task<List<Feeling>> GetFeelingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}

