namespace MeTenTenMaui.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> SignUpAsync(string email, string password, string name);
        Task<(bool Success, string? ErrorMessage)> SignInAsync(string email, string password);
        Task SignOutAsync();
        Task<bool> TryAutoLoginAsync();
        bool IsAuthenticated { get; }
        string? CurrentUserId { get; }
        string? CurrentUserEmail { get; }
        string? CurrentUserName { get; }
    }
}

