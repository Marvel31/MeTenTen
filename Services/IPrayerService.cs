using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public interface IPrayerService
    {
        Task<List<Prayer>> GetAllPrayersAsync();
        Task<Prayer?> GetPrayerByIdAsync(int id);
        Task<List<Prayer>> GetPrayersByCategoryAsync(string category);
        Task<List<Prayer>> GetFavoritePrayersAsync();
        Task<Prayer> CreatePrayerAsync(Prayer prayer);
        Task<Prayer> UpdatePrayerAsync(Prayer prayer);
        Task DeletePrayerAsync(int id);
        Task ToggleFavoriteAsync(int id);
    }
}

