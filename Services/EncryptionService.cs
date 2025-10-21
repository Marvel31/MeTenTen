using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeTenTenMaui.Services
{
    public class EncryptionService : IEncryptionService
    {
        private byte[]? _dek;
        private byte[]? _sharedDek; // Data Encryption Key (실제 데이터 암호화에 사용)
        private const int KeySize = 256 / 8; // 32 bytes for AES-256
        private const int Iterations = 100000;
        private const string AppSalt = "MeTenTen2025";
        private const int IVSize = 16; // 128 bits for AES

        public bool IsInitialized => _dek != null;
        public bool HasSharedDEK => _sharedDek != null;

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
                System.Diagnostics.Debug.WriteLine("[Encryption] Service not initialized");
                throw new InvalidOperationException("Encryption service not initialized. Call SetDEK first.");
            }

            if (string.IsNullOrEmpty(encryptedText))
            {
                System.Diagnostics.Debug.WriteLine("[Encryption] Encrypted text is empty");
                return Task.FromResult(string.Empty);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Starting decryption of {encryptedText.Length} chars");
                var fullCipher = Convert.FromBase64String(encryptedText);
                System.Diagnostics.Debug.WriteLine($"[Encryption] Base64 decoded to {fullCipher.Length} bytes");

                if (fullCipher.Length < IVSize)
                {
                    System.Diagnostics.Debug.WriteLine($"[Encryption] Invalid data length: {fullCipher.Length} < {IVSize}");
                    throw new ArgumentException("Invalid encrypted data");
                }

                // IV 추출
                var iv = new byte[IVSize];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, IVSize);

                // 암호문 추출
                var cipherBytes = new byte[fullCipher.Length - IVSize];
                Buffer.BlockCopy(fullCipher, IVSize, cipherBytes, 0, cipherBytes.Length);

                System.Diagnostics.Debug.WriteLine($"[Encryption] IV: {Convert.ToBase64String(iv)}, Cipher length: {cipherBytes.Length}");

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
                        System.Diagnostics.Debug.WriteLine($"[Encryption] Successfully decrypted {encryptedText.Length} chars to {result.Length} chars");
                        return Task.FromResult(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Decryption error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Encryption] Exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[Encryption] Stack trace: {ex.StackTrace}");
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


        public async Task<byte[]> GenerateSharedDEKAsync()
        {
            return await GenerateRandomDEKAsync();
        }


        public void SetSharedDEK(byte[] sharedDek)
        {
            if (sharedDek == null || sharedDek.Length != KeySize)
            {
                throw new ArgumentException("Invalid shared DEK");
            }

            _sharedDek = new byte[sharedDek.Length];
            Buffer.BlockCopy(sharedDek, 0, _sharedDek, 0, sharedDek.Length);
            System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK set in memory");
        }

        public void ClearSharedDEK()
        {
            if (_sharedDek != null)
            {
                Array.Clear(_sharedDek, 0, _sharedDek.Length);
                _sharedDek = null;
            }
            System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK cleared from memory");
        }

        public async Task<string> EncryptWithSharedDEKAsync(string plainText)
        {
            if (!HasSharedDEK)
            {
                throw new InvalidOperationException("Shared DEK not initialized. Call SetSharedDEK first.");
            }

            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _sharedDek!;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateIV(); // 랜덤 IV 생성

                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    {
                        // IV를 앞에 추가
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            await swEncrypt.WriteAsync(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Error encrypting with shared DEK: {ex.Message}");
                throw new InvalidOperationException("Failed to encrypt with shared DEK", ex);
            }
        }

        public async Task<string> DecryptWithSharedDEKAsync(string encryptedText)
        {
            if (!HasSharedDEK)
            {
                System.Diagnostics.Debug.WriteLine("[Encryption] Shared DEK not initialized");
                throw new InvalidOperationException("Shared DEK not initialized. Call SetSharedDEK first.");
            }

            if (string.IsNullOrEmpty(encryptedText))
            {
                System.Diagnostics.Debug.WriteLine("[Encryption] Shared DEK encrypted text is empty");
                return string.Empty;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Starting shared DEK decryption of {encryptedText.Length} chars");
                var cipherBytes = Convert.FromBase64String(encryptedText);
                System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK base64 decoded to {cipherBytes.Length} bytes");

                using (var aes = Aes.Create())
                {
                    aes.Key = _sharedDek!;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // IV 추출 (처음 16바이트)
                    var iv = new byte[16];
                    Array.Copy(cipherBytes, 0, iv, 0, 16);
                    aes.IV = iv;

                    // 암호화된 데이터 추출 (IV 이후부터)
                    var encryptedData = new byte[cipherBytes.Length - 16];
                    Array.Copy(cipherBytes, 16, encryptedData, 0, encryptedData.Length);

                    System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK IV: {Convert.ToBase64String(iv)}, Encrypted data length: {encryptedData.Length}");

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(encryptedData))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        var result = await srDecrypt.ReadToEndAsync();
                        System.Diagnostics.Debug.WriteLine($"[Encryption] Successfully decrypted with shared DEK {encryptedText.Length} chars to {result.Length} chars");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Encryption] Error decrypting with shared DEK: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[Encryption] Shared DEK stack trace: {ex.StackTrace}");
                throw new InvalidOperationException("Failed to decrypt with shared DEK", ex);
            }
        }
    }
}
