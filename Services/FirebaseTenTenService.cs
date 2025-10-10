using MeTenTenMaui.Models;

namespace MeTenTenMaui.Services
{
    public class FirebaseTenTenService : ITenTenService
    {
        private readonly IFirebaseDataService _firebaseDataService;
        private readonly IAuthService _authService;
        private List<TenTen> _cachedTenTens = new();
        private int _nextId = 1;

        public FirebaseTenTenService(IFirebaseDataService firebaseDataService, IAuthService authService)
        {
            _firebaseDataService = firebaseDataService;
            _authService = authService;
        }

        public async Task<List<TenTen>> GetTenTensAsync()
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                return new List<TenTen>();
            }

            _cachedTenTens = await _firebaseDataService.GetTenTensAsync(_authService.CurrentUserId);
            
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

            return await _firebaseDataService.GetTenTensByTopicAsync(_authService.CurrentUserId, topicId);
        }

        public async Task<TenTen?> GetTenTenByIdAsync(int id)
        {
            var tenTens = await GetAllTenTensAsync();
            return tenTens.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TenTen> CreateTenTenAsync(CreateTenTenRequest request)
        {
            if (string.IsNullOrEmpty(_authService.CurrentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            return await _firebaseDataService.CreateTenTenAsync(_authService.CurrentUserId, request);
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

