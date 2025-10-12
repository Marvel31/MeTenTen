using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeTenTenMaui.Services
{
    public class EncryptionService : IEncryptionService
    {
        private byte[]? _key;
        private const int KeySize = 256 / 8; // 32 bytes for AES-256
        private const int Iterations = 100000;
        private const string AppSalt = "MeTenTen2025";
        private const int IVSize = 16; // 128 bits for AES

        public bool IsInitialized => _key != null;

        public Task InitializeAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password cannot be empty");
            }

            // PBKDF2로 키 생성: 사용자 이메일 + 고정 앱 Salt
            var salt = Encoding.UTF8.GetBytes(email + AppSalt);
            
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                _key = pbkdf2.GetBytes(KeySize);
            }

            System.Diagnostics.Debug.WriteLine($"[Encryption] Key initialized for user: {email}");
            return Task.CompletedTask;
        }

        public Task<string> EncryptAsync(string plainText)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Encryption service not initialized. Call InitializeAsync first.");
            }

            if (string.IsNullOrEmpty(plainText))
            {
                return Task.FromResult(string.Empty);
            }

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _key!;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateIV(); // 랜덤 IV 생성

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var plainBytes = Encoding.UTF8.GetBytes(plainText);
                        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        
                        // IV + 암호문 결합
                        var result = new byte[IVSize + encryptedBytes.Length];
                        Buffer.BlockCopy(aes.IV, 0, result, 0, IVSize);
                        Buffer.BlockCopy(encryptedBytes, 0, result, IVSize, encryptedBytes.Length);
                        
                        var base64Result = Convert.ToBase64String(result);
                        System.Diagnostics.Debug.WriteLine($"[Encryption] Encrypted {plainText.Length} chars to {base64Result.Length} chars");
                        return Task.FromResult(base64Result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Encryption error: {ex.Message}");
                throw new InvalidOperationException("Failed to encrypt data", ex);
            }
        }

        public Task<string> DecryptAsync(string encryptedText)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Encryption service not initialized. Call InitializeAsync first.");
            }

            if (string.IsNullOrEmpty(encryptedText))
            {
                return Task.FromResult(string.Empty);
            }

            try
            {
                var fullCipher = Convert.FromBase64String(encryptedText);

                if (fullCipher.Length < IVSize)
                {
                    throw new ArgumentException("Invalid encrypted data");
                }

                // IV 추출
                var iv = new byte[IVSize];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, IVSize);

                // 암호문 추출
                var cipherBytes = new byte[fullCipher.Length - IVSize];
                Buffer.BlockCopy(fullCipher, IVSize, cipherBytes, 0, cipherBytes.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key = _key!;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        var result = Encoding.UTF8.GetString(decryptedBytes);
                        System.Diagnostics.Debug.WriteLine($"[Encryption] Decrypted {encryptedText.Length} chars to {result.Length} chars");
                        return Task.FromResult(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Decryption error: {ex.Message}");
                throw new InvalidOperationException("Failed to decrypt data. The data may be corrupted or the password is incorrect.", ex);
            }
        }

        public void ClearKey()
        {
            if (_key != null)
            {
                // 보안을 위해 키를 0으로 덮어쓰기
                Array.Clear(_key, 0, _key.Length);
                _key = null;
                System.Diagnostics.Debug.WriteLine("[Encryption] Key cleared from memory");
            }
        }
    }
}

