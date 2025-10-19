using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class FirebaseTenTenService : ITenTenService
    {
        private readonly IFirebaseDataService _firebaseDataService;
        private readonly IAuthService _authService;
        private readonly IEncryptionService _encryptionService;
        private List<TenTen> _cachedTenTens = new();
        private int _nextId = 1;

        public FirebaseTenTenService(IFirebaseDataService firebaseDataService, IAuthService authService, IEncryptionService encryptionService)
        {
            _firebaseDataService = firebaseDataService;
            _authService = authService;
            _encryptionService = encryptionService;
        }

        public async Task<List<TenTen>> GetTenTensAsync()
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return new List<TenTen>();
            }

            _cachedTenTens = await _firebaseDataService.GetTenTensAsync(_authService.CurrentUserId);
            
            // 암호화된 데이터 복호화
            foreach (var tenTen in _cachedTenTens)
            {
                if (tenTen.IsEncrypted && !string.IsNullOrEmpty(tenTen.Content))
                {
                    try
                    {
                        if (tenTen.EncryptionType == "shared" && _encryptionService.HasSharedDEK)
                        {
                            tenTen.Content = await _encryptionService.DecryptWithSharedDEKAsync(tenTen.Content);
                        }
                        else if (tenTen.EncryptionType == "shared" && !_encryptionService.HasSharedDEK)
                        {
                            // Shared DEK가 없는 경우 개인 DEK로 복호화 시도
                            System.Diagnostics.Debug.WriteLine($"[TenTen] Shared DEK not available, trying personal DEK for ID {tenTen.Id}");
                            tenTen.Content = await _encryptionService.DecryptAsync(tenTen.Content);
                        }
                        else
                        {
                            tenTen.Content = await _encryptionService.DecryptAsync(tenTen.Content);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[TenTen] Decrypt error for ID {tenTen.Id}: {ex.Message}");
                        tenTen.Content = "[복호화 실패]";
                    }
                }
            }
            
            // ID 재할당
            _nextId = 1;
            foreach (var tenTen in _cachedTenTens.OrderBy(t => t.CreatedAt))
            {
                tenTen.Id = _nextId++;
            }

            return _cachedTenTens;
        }

        public async Task<List<TenTen>> GetAllTenTensAsync()
        {
            return await GetTenTensAsync();
        }

        public async Task<List<TenTen>> GetTenTensByTopicAsync(int topicId)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return new List<TenTen>();
            }

            // Topic의 Firebase 키를 찾기 위해 모든 Topic을 조회
            var topics = await _firebaseDataService.GetTopicsAsync(_authService.CurrentUserId);
            var topic = topics.FirstOrDefault(t => t.Id == topicId);
            
            if (topic == null || string.IsNullOrEmpty(topic.FirebaseKey))
            {
                return new List<TenTen>();
            }

            var tenTens = await _firebaseDataService.GetTenTensByTopicAsync(_authService.CurrentUserId, topic.FirebaseKey);
            
            // 암호화된 데이터 복호화
            foreach (var tenTen in tenTens)
            {
                if (tenTen.IsEncrypted && !string.IsNullOrEmpty(tenTen.Content))
                {
                    try
                    {
                        if (tenTen.EncryptionType == "shared" && _encryptionService.HasSharedDEK)
                        {
                            tenTen.Content = await _encryptionService.DecryptWithSharedDEKAsync(tenTen.Content);
                        }
                        else if (tenTen.EncryptionType == "shared" && !_encryptionService.HasSharedDEK)
                        {
                            // Shared DEK가 없는 경우 개인 DEK로 복호화 시도
                            System.Diagnostics.Debug.WriteLine($"[TenTen] Shared DEK not available, trying personal DEK for ID {tenTen.Id}");
                            tenTen.Content = await _encryptionService.DecryptAsync(tenTen.Content);
                        }
                        else
                        {
                            tenTen.Content = await _encryptionService.DecryptAsync(tenTen.Content);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[TenTen] Decrypt error for ID {tenTen.Id}: {ex.Message}");
                        tenTen.Content = "[복호화 실패]";
                    }
                }
            }
            
            return tenTens;
        }

        public async Task<TenTen?> GetTenTenByIdAsync(int id)
        {
            var tenTens = await GetAllTenTensAsync();
            return tenTens.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
        {
            return await CreateTenTenAsync(request, "personal");
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request, string encryptionType)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            return await _firebaseDataService.CreateTenTenAsync(_authService.CurrentUserId, request, encryptionType);
        }

        public async Task<TenTen> UpdateTenTenAsync(int id, UpdateTenTenRequest request)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var existingTenTen = _cachedTenTens.FirstOrDefault(t => t.Id == id);
            if (existingTenTen == null || string.IsNullOrEmpty(existingTenTen.FirebaseKey))
            {
                throw new ArgumentException($"TenTen with ID {id} not found");
            }

            return await _firebaseDataService.UpdateTenTenAsync(_authService.CurrentUserId, existingTenTen.FirebaseKey, request);
        }

        public async Task<TenTen> UpdateTenTenAsync(TenTen tenTen)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            if (string.IsNullOrEmpty(tenTen.FirebaseKey))
            {
                throw new ArgumentException("FirebaseKey is required");
            }

            var updateRequest = new UpdateTenTenRequest
            {
                Content = tenTen.Content
            };

            return await _firebaseDataService.UpdateTenTenAsync(_authService.CurrentUserId, tenTen.FirebaseKey, updateRequest);
        }

        public async Task<bool> DeleteTenTenAsync(int id)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return false;
            }

            var existingTenTen = _cachedTenTens.FirstOrDefault(t => t.Id == id);
            if (existingTenTen == null || string.IsNullOrEmpty(existingTenTen.FirebaseKey))
            {
                return false;
            }

            return await _firebaseDataService.DeleteTenTenAsync(_authService.CurrentUserId, existingTenTen.FirebaseKey);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            // Firebase에서는 아직 미구현
            await Task.CompletedTask;
            return true;
        }
    }
}

