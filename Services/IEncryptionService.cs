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
        
        // 공유 DEK 생성 (배우자 간 공유용)
        Task<byte[]> GenerateSharedDEKAsync();
        
        
        // 공유 DEK 관리
        void SetSharedDEK(byte[] sharedDek);
        void ClearSharedDEK();
        bool HasSharedDEK { get; }
        
        // 공유 DEK로 암호화
        Task<string> EncryptWithSharedDEKAsync(string plainText);
        
        // 공유 DEK로 복호화
        Task<string> DecryptWithSharedDEKAsync(string encryptedText);
    }
}

