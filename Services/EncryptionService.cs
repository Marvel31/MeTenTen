using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeTenTenMaui.Services
{
    public class EncryptionService : IEncryptionService
    {
        private byte[]? _dek; // Data Encryption Key (실제 데이터 암호화에 사용)
        private const int KeySize = 256 / 8; // 32 bytes for AES-256
        private const int Iterations = 100000;
        private const string AppSalt = "MeTenTen2025";
        private const int IVSize = 16; // 128 bits for AES

        public bool IsInitialized => _dek != null;

        /// <summary>
        /// 랜덤한 DEK 생성 (회원가입 시 1회만)
        /// </summary>
        public Task<byte[]> GenerateRandomDEKAsync()
        {
            var dek = new byte[KeySize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(dek);
            }
            System.Diagnostics.Debug.WriteLine($"[Encryption] Generated random DEK: {dek.Length} bytes");
            return Task.FromResult(dek);
        }

        /// <summary>
        /// DEK를 사용자 비밀번호로 암호화 (Firebase 저장용)
        /// </summary>
        public Task<string> EncryptDEKAsync(byte[] dek, string email, string password)
        {
            if (dek == null || dek.Length != KeySize)
            {
                throw new ArgumentException("Invalid DEK");
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password cannot be empty");
            }

            try
            {
                // 비밀번호로 암호화 키 생성 (PBKDF2)
                var salt = Encoding.UTF8.GetBytes(email + AppSalt);
                byte[] passwordKey;
                
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    passwordKey = pbkdf2.GetBytes(KeySize);
                }

                // DEK를 비밀번호 키로 암호화
                using (var aes = Aes.Create())
                {
                    aes.Key = passwordKey;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var encryptedDek = encryptor.TransformFinalBlock(dek, 0, dek.Length);
                        
                        // IV + 암호화된 DEK 결합
                        var result = new byte[IVSize + encryptedDek.Length];
                        Buffer.BlockCopy(aes.IV, 0, result, 0, IVSize);
                        Buffer.BlockCopy(encryptedDek, 0, result, IVSize, encryptedDek.Length);
                        
                        var base64Result = Convert.ToBase64String(result);
                        System.Diagnostics.Debug.WriteLine($"[Encryption] DEK encrypted with password");
                        return Task.FromResult(base64Result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] DEK encryption error: {ex.Message}");
                throw new InvalidOperationException("Failed to encrypt DEK", ex);
            }
        }

        /// <summary>
        /// 암호화된 DEK를 사용자 비밀번호로 복호화 (로그인 시)
        /// </summary>
        public Task<byte[]> DecryptDEKAsync(string encryptedDek, string email, string password)
        {
            if (string.IsNullOrEmpty(encryptedDek))
            {
                throw new ArgumentException("Encrypted DEK cannot be empty");
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password cannot be empty");
            }

            try
            {
                // 비밀번호로 암호화 키 생성 (PBKDF2)
                var salt = Encoding.UTF8.GetBytes(email + AppSalt);
                byte[] passwordKey;
                
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    passwordKey = pbkdf2.GetBytes(KeySize);
                }

                var fullCipher = Convert.FromBase64String(encryptedDek);

                if (fullCipher.Length < IVSize)
                {
                    throw new ArgumentException("Invalid encrypted DEK data");
                }

                // IV 추출
                var iv = new byte[IVSize];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, IVSize);

                // 암호화된 DEK 추출
                var cipherBytes = new byte[fullCipher.Length - IVSize];
                Buffer.BlockCopy(fullCipher, IVSize, cipherBytes, 0, cipherBytes.Length);

                // DEK 복호화
                using (var aes = Aes.Create())
                {
                    aes.Key = passwordKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedDek = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        System.Diagnostics.Debug.WriteLine($"[Encryption] DEK decrypted successfully");
                        return Task.FromResult(decryptedDek);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] DEK decryption error: {ex.Message}");
                throw new InvalidOperationException("Failed to decrypt DEK. The password may be incorrect.", ex);
            }
        }

        /// <summary>
        /// DEK 설정 (로그인 성공 후 호출)
        /// </summary>
        public void SetDEK(byte[] dek)
        {
            if (dek == null || dek.Length != KeySize)
            {
                throw new ArgumentException("Invalid DEK");
            }

            _dek = new byte[dek.Length];
            Buffer.BlockCopy(dek, 0, _dek, 0, dek.Length);
            System.Diagnostics.Debug.WriteLine($"[Encryption] DEK set in memory");
        }

        /// <summary>
        /// 데이터 암호화 (DEK 사용)
        /// </summary>
        public Task<string> EncryptAsync(string plainText)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Encryption service not initialized. Call SetDEK first.");
            }

            if (string.IsNullOrEmpty(plainText))
            {
                return Task.FromResult(string.Empty);
            }

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _dek!;
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

        /// <summary>
        /// 데이터 복호화 (DEK 사용)
        /// </summary>
        public Task<string> DecryptAsync(string encryptedText)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Encryption service not initialized. Call SetDEK first.");
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
                    aes.Key = _dek!;
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
                throw new InvalidOperationException("Failed to decrypt data. The data may be corrupted or the DEK is incorrect.", ex);
            }
        }

        /// <summary>
        /// DEK를 메모리에서 안전하게 제거 (로그아웃 시)
        /// </summary>
        public void ClearKey()
        {
            if (_dek != null)
            {
                // 보안을 위해 DEK를 0으로 덮어쓰기
                Array.Clear(_dek, 0, _dek.Length);
                _dek = null;
                System.Diagnostics.Debug.WriteLine("[Encryption] DEK cleared from memory");
            }
        }
    }
}
