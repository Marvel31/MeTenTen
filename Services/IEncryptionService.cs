using System.Threading.Tasks;

namespace MeTenTenMaui.Services
{
    public interface IEncryptionService
    {
        Task InitializeAsync(string email, string password);
        Task<string> EncryptAsync(string plainText);
        Task<string> DecryptAsync(string encryptedText);
        void ClearKey();
        bool IsInitialized { get; }
    }
}

