using Firebase.Database;
using Firebase.Database.Query;
using MeTenTenMaui.Models;
using MeTenTenMaui.Models.Firebase;

namespace MeTenTenMaui.Services
{
    public class FirebaseDataService : IFirebaseDataService
    {
        private const string FirebaseUrl = "https://metenten-d4a6f-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private readonly FirebaseClient _firebaseClient;
        private readonly IEncryptionService _encryptionService;

        public FirebaseDataService(IEncryptionService encryptionService)
        {
            _firebaseClient = new FirebaseClient(FirebaseUrl);
            _encryptionService = encryptionService;
        }

        #region User DEK Management

        public async Task SaveUserDEKAsync(string userId, string email, string displayName, string encryptedDEK)
        {
            try
            {
                var firebaseUser = new FirebaseUser
                {
                    Email = email,
                    DisplayName = displayName,
                    EncryptedDEK = encryptedDEK,
                    CreatedAt = DateTime.Now.ToString("o"),
                    UpdatedAt = DateTime.Now.ToString("o")
                };

                await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .PutAsync(firebaseUser);

                System.Diagnostics.Debug.WriteLine($"[Firebase] Saved encrypted DEK for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error saving user DEK: {ex.Message}");
                throw;
            }
        }

        public async Task<string?> GetUserDEKAsync(string userId)
        {
            try
            {
                var firebaseUser = await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .OnceSingleAsync<FirebaseUser>();

                if (firebaseUser != null && !string.IsNullOrEmpty(firebaseUser.EncryptedDEK))
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Retrieved encrypted DEK for user: {userId}");
                    return firebaseUser.EncryptedDEK;
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] No encrypted DEK found for user: {userId}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting user DEK: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Topic CRUD

        public async Task<List<Topic>> GetTopicsAsync(string userId)
        {
            try
            {
                var firebaseTopics = await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .OnceAsync<FirebaseTopic>();

                var topics = new List<Topic>();
                int idCounter = 1;

                foreach (var item in firebaseTopics.Where(t => t.Object.IsActive))
                {
                    topics.Add(ConvertFromFirebase(item.Object, item.Key, idCounter++));
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Loaded {topics.Count} topics for user {userId}");
                return topics;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error loading topics: {ex.Message}");
                return new List<Topic>();
            }
        }

        public async Task<Topic?> GetTopicByIdAsync(string userId, string topicId)
        {
            try
            {
                var firebaseTopic = await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .Child(topicId)
                    .OnceSingleAsync<FirebaseTopic>();

                if (firebaseTopic != null)
                {
                    return ConvertFromFirebase(firebaseTopic, topicId, 0);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting topic {topicId}: {ex.Message}");
                return null;
            }
        }

        public async Task<Topic> CreateTopicAsync(string userId, CreateTopicRequest request)
        {
            try
            {
                var firebaseTopic = new FirebaseTopic
                {
                    Subject = request.Subject,
                    Description = request.Description,
                    TopicDate = request.TopicDate.ToString("yyyy-MM-dd"),
                    CreatedAt = DateTime.Now.ToString("o"),
                    IsActive = true
                };

                var result = await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .PostAsync(firebaseTopic);

                firebaseTopic.Id = result.Key;
                
                var topic = ConvertFromFirebase(firebaseTopic, result.Key, 0);
                System.Diagnostics.Debug.WriteLine($"[Firebase] Created topic: {topic.Subject}");
                return topic;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error creating topic: {ex.Message}");
                throw;
            }
        }

        public async Task<Topic> UpdateTopicAsync(string userId, string topicId, UpdateTopicRequest request)
        {
            try
            {
                // Patch로 필요한 필드만 업데이트
                var updates = new
                {
                    subject = request.Subject,
                    description = request.Description,
                    topicDate = request.TopicDate.ToString("yyyy-MM-dd"),
                    updatedAt = DateTime.Now.ToString("o"),
                    isActive = request.IsActive
                };

                await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .Child(topicId)
                    .PatchAsync(updates);

                // 업데이트된 전체 데이터를 다시 가져오기
                var updatedTopic = await GetTopicByIdAsync(userId, topicId);
                
                if (updatedTopic != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Updated topic: {updatedTopic.Subject}");
                    return updatedTopic;
                }

                throw new Exception("Updated topic not found");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating topic {topicId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTopicAsync(string userId, string topicId)
        {
            try
            {
                // Soft delete - IsActive를 false로 설정
                await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .Child(topicId)
                    .Child("isActive")
                    .PutAsync(false);

                System.Diagnostics.Debug.WriteLine($"[Firebase] Deleted topic: {topicId}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error deleting topic {topicId}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region TenTen CRUD

        public async Task<List<TenTen>> GetTenTensAsync(string userId)
        {
            try
            {
                var firebaseTenTens = await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .OnceAsync<FirebaseTenTen>();

                var tenTens = new List<TenTen>();
                int idCounter = 1;

                foreach (var item in firebaseTenTens)
                {
                    tenTens.Add(ConvertFromFirebase(item.Object, item.Key, idCounter++));
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Loaded {tenTens.Count} tentens for user {userId}");
                return tenTens;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error loading tentens: {ex.Message}");
                return new List<TenTen>();
            }
        }

        public async Task<TenTen?> GetTenTenByIdAsync(string userId, string tenTenId)
        {
            try
            {
                var firebaseTenTen = await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .Child(tenTenId)
                    .OnceSingleAsync<FirebaseTenTen>();

                if (firebaseTenTen != null)
                {
                    return ConvertFromFirebase(firebaseTenTen, tenTenId, 0);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting tenten {tenTenId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TenTen>> GetTenTensByTopicAsync(string userId, int topicId)
        {
            try
            {
                var allTenTens = await GetTenTensAsync(userId);
                return allTenTens.Where(t => t.TopicId == topicId).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting tentens by topic {topicId}: {ex.Message}");
                return new List<TenTen>();
            }
        }

        public async Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request)
        {
            try
            {
                // Content 암호화
                var encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                
                var firebaseTenTen = new FirebaseTenTen
                {
                    TopicId = request.TopicId.ToString(),
                    Content = encryptedContent,
                    IsEncrypted = true,
                    CreatedAt = DateTime.Now.ToString("o")
                };

                var result = await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .PostAsync(firebaseTenTen);

                firebaseTenTen.Id = result.Key;

                var tenTen = ConvertFromFirebase(firebaseTenTen, result.Key, 0);
                // UI 표시를 위해 복호화된 Content 반환
                tenTen.Content = request.Content;
                System.Diagnostics.Debug.WriteLine($"[Firebase] Created tenten for topic {request.TopicId}");
                return tenTen;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error creating tenten: {ex.Message}");
                throw;
            }
        }

        public async Task<TenTen> UpdateTenTenAsync(string userId, string tenTenId, UpdateTenTenRequest request)
        {
            try
            {
                // Content 암호화
                var encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                
                var updates = new
                {
                    content = encryptedContent,
                    isEncrypted = true,
                    updatedAt = DateTime.Now.ToString("o")
                };

                await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .Child(tenTenId)
                    .PatchAsync(updates);

                // 업데이트된 데이터 반환
                var updatedTenTen = await GetTenTenByIdAsync(userId, tenTenId);
                if (updatedTenTen != null)
                {
                    // UI 표시를 위해 복호화된 Content 반환
                    updatedTenTen.Content = request.Content;
                }
                System.Diagnostics.Debug.WriteLine($"[Firebase] Updated tenten: {tenTenId}");
                return updatedTenTen ?? throw new Exception("Updated TenTen not found");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating tenten {tenTenId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTenTenAsync(string userId, string tenTenId)
        {
            try
            {
                await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .Child(tenTenId)
                    .DeleteAsync();

                System.Diagnostics.Debug.WriteLine($"[Firebase] Deleted tenten: {tenTenId}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error deleting tenten {tenTenId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTenTensByTopicAsync(string userId, int topicId)
        {
            try
            {
                var tenTens = await GetTenTensByTopicAsync(userId, topicId);
                
                foreach (var tenTen in tenTens)
                {
                    if (!string.IsNullOrEmpty(tenTen.TopicSubject))
                    {
                        // Firebase Key는 TopicSubject에 임시로 저장됨
                        await DeleteTenTenAsync(userId, tenTen.TopicSubject);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Deleted {tenTens.Count} tentens for topic {topicId}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error deleting tentens by topic {topicId}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Helper Methods

        private Topic ConvertFromFirebase(FirebaseTopic firebaseTopic, string firebaseKey, int localId)
        {
            return new Topic
            {
                Id = localId,
                FirebaseKey = firebaseKey,
                Subject = firebaseTopic.Subject ?? string.Empty,
                Description = firebaseTopic.Description ?? string.Empty,
                TopicDate = ParseDate(firebaseTopic.TopicDate, DateTime.Today),
                CreatedAt = ParseDate(firebaseTopic.CreatedAt, DateTime.Now),
                UpdatedAt = string.IsNullOrEmpty(firebaseTopic.UpdatedAt) ? null : ParseDate(firebaseTopic.UpdatedAt, null),
                IsActive = firebaseTopic.IsActive
            };
        }

        private DateTime ParseDate(string? dateString, DateTime? defaultValue)
        {
            if (string.IsNullOrEmpty(dateString))
                return defaultValue ?? DateTime.Now;

            if (DateTime.TryParse(dateString, out DateTime result))
                return result;

            return defaultValue ?? DateTime.Now;
        }

        private TenTen ConvertFromFirebase(FirebaseTenTen firebaseTenTen, string firebaseKey, int localId)
        {
            return new TenTen
            {
                Id = localId,
                FirebaseKey = firebaseKey,
                TopicId = int.TryParse(firebaseTenTen.TopicId, out int topicId) ? topicId : 0,
                Content = firebaseTenTen.Content ?? string.Empty,
                CreatedAt = ParseDate(firebaseTenTen.CreatedAt, DateTime.Now),
                UpdatedAt = string.IsNullOrEmpty(firebaseTenTen.UpdatedAt) ? null : ParseDate(firebaseTenTen.UpdatedAt, null),
                UserId = 1,
                UserName = "현재 사용자",
                TopicSubject = "",
                IsReadByPartner = false,
                IsEncrypted = firebaseTenTen.IsEncrypted
            };
        }

        #endregion
    }
}

