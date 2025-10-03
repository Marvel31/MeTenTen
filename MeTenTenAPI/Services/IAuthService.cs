using MeTenTenAPI.DTOs;

namespace MeTenTenAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(CreateUserDto createUserDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> LinkPartnerAsync(int userId, LinkPartnerDto linkPartnerDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
    }
}

