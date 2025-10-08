using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface IFileStorageService
    {
        Task SaveTenTensAsync(List<TenTen> tenTens);
        Task<List<TenTen>> LoadTenTensAsync();
        Task SaveTopicsAsync(List<Topic> topics);
        Task<List<Topic>> LoadTopicsAsync();
        Task<string> ExportMonthDataAsync(int year, int month, List<TenTen> tenTens, List<Topic> topics);
        Task<(List<TenTen> tenTens, List<Topic> topics)> ImportDataAsync(string filePath);
        Task SaveFeelingExamplesAsync(List<FeelingExample> examples);
        Task<List<FeelingExample>> LoadFeelingExamplesAsync();
    }
}

