using System.Threading.Tasks;

namespace MeTenTenMaui.Services
{
    public interface IEncryptionService
    {
        // DEK (Data Encryption Key) 관리
        Task<byte[]> GenerateRandomDEKAsync();
        Task<string> EncryptDEKAsync(byte[] dek, string email, string password);
        Task<byte[]> DecryptDEKAsync(string encryptedDek, string email, string password);
        
        // DEK 설정 (로그인 시 사용)
        void SetDEK(byte[] dek);
        
        // 데이터 암호화/복호화 (DEK 사용)
        Task<string> EncryptAsync(string plainText);
        Task<string> DecryptAsync(string encryptedText);
        
        // 키 관리
        void ClearKey();
        bool IsInitialized { get; }
    }
}

