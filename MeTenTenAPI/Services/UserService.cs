using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using MeTenTenAPI.Data;
using MeTenTenAPI.DTOs;
using MeTenTenAPI.Models;

namespace MeTenTenAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("사용자를 찾을 수 없습니다.");
            }

            if (!string.IsNullOrEmpty(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                // 이메일 중복 확인
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != userId))
                {
                    throw new InvalidOperationException("이미 사용 중인 이메일입니다.");
                }
                user.Email = updateUserDto.Email;
            }

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                PartnerId = user.PartnerId,
                PartnerName = user.Partner?.Name
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            // 현재 비밀번호 확인
            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // 새 비밀번호로 변경
            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDto?> GetPartnerAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Partner == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Partner.Id,
                Name = user.Partner.Name,
                Email = user.Partner.Email,
                CreatedAt = user.Partner.CreatedAt,
                LastLoginAt = user.Partner.LastLoginAt,
                PartnerId = user.Partner.PartnerId,
                PartnerName = user.Name
            };
        }

        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[32];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[64];
            Array.Copy(salt, 0, hashBytes, 0, 32);
            Array.Copy(hash, 0, hashBytes, 32, 32);

            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            var salt = new byte[32];
            Array.Copy(hashBytes, 0, salt, 0, 32);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 32] != hash[i])
                    return false;
            }

            return true;
        }
    }
}

