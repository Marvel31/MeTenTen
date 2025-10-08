using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface IFeelingExampleService
    {
        Task<List<FeelingExample>> GetAllExamplesAsync();
        Task<List<FeelingExample>> GetExamplesByCategoryAsync(string category);
        Task<FeelingExample> AddExampleAsync(FeelingExample example);
        Task<bool> DeleteExampleAsync(int id);
    }
}

