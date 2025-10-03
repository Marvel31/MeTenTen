using MeTenTenAPI.DTOs;

namespace MeTenTenAPI.Services
{
    public interface IUserService
    {
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<UserDto?> GetPartnerAsync(int userId);
    }
}

