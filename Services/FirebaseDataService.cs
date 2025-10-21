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
                System.Diagnostics.Debug.WriteLine($"[Firebase] Getting user DEK for user: {userId}");
                
                var firebaseUser = await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .OnceSingleAsync<FirebaseUser>();

                if (firebaseUser != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User object retrieved successfully");
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User email: {firebaseUser.Email}");
                    System.Diagnostics.Debug.WriteLine($"[Firebase] EncryptedDEK exists: {!string.IsNullOrEmpty(firebaseUser.EncryptedDEK)}");
                    
                    if (!string.IsNullOrEmpty(firebaseUser.EncryptedDEK))
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Retrieved encrypted DEK for user: {userId}");
                        return firebaseUser.EncryptedDEK;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] EncryptedDEK is null or empty for user: {userId}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User object is null for user: {userId}");
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] No encrypted DEK found for user: {userId}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting user DEK: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Inner exception: {ex.InnerException.Message}");
                }
                
                // JSON 파싱 실패 시 raw 데이터로 재시도
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Attempting to get raw user data for debugging");
                    var rawData = await _firebaseClient
                        .Child("users")
                        .Child(userId)
                        .OnceSingleAsync<dynamic>();
                    
                    if (rawData != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Raw data retrieved, checking EncryptedDEK field");
                        var encryptedDEK = rawData.EncryptedDEK?.ToString();
                        if (!string.IsNullOrEmpty(encryptedDEK))
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Found EncryptedDEK in raw data");
                            return encryptedDEK;
                        }
                    }
                }
                catch (Exception rawEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Raw data retrieval also failed: {rawEx.Message}");
                }
                
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

        public async Task<List<Topic>> GetAllTopicsAsync(string userId)
        {
            try
            {
                var firebaseTopics = await _firebaseClient
                    .Child("topics")
                    .Child(userId)
                    .OnceAsync<FirebaseTopic>();

                var topics = new List<Topic>();
                int idCounter = 1;

                foreach (var item in firebaseTopics)
                {
                    topics.Add(ConvertFromFirebase(item.Object, item.Key, idCounter++));
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Loaded {topics.Count} all topics for user {userId}");
                return topics;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error loading all topics: {ex.Message}");
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

        public async Task<List<TenTen>> GetTenTensByTopicAsync(string userId, string topicFirebaseKey)
        {
            try
            {
                var firebaseTenTens = await _firebaseClient
                    .Child("tentens")
                    .Child(userId)
                    .OnceAsync<FirebaseTenTen>();

                var tenTens = new List<TenTen>();
                int idCounter = 1;

                // Topic 정보 조회 (int ID와 Firebase Key 매핑용)
                var topics = await GetTopicsAsync(userId);
                var topic = topics.FirstOrDefault(t => t.FirebaseKey == topicFirebaseKey);
                
                foreach (var item in firebaseTenTens)
                {
                    // Firebase Key로 직접 매칭 또는 int ID로 매칭
                    bool isMatch = item.Object.TopicId == topicFirebaseKey;
                    
                    // 기존 데이터 호환성을 위해 int ID로도 매칭 시도
                    if (!isMatch && topic != null)
                    {
                        isMatch = item.Object.TopicId == topic.Id.ToString();
                        
                        // 매칭되면 Firebase Key로 업데이트
                        if (isMatch)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Migrating TenTen {item.Key} from int ID to Firebase Key");
                            try
                            {
                                // 전체 TenTen 객체를 업데이트하여 topicId 필드 변경
                                var updatedTenTen = item.Object;
                                updatedTenTen.TopicId = topicFirebaseKey;
                                
                                await _firebaseClient
                                    .Child("tentens")
                                    .Child(userId)
                                    .Child(item.Key)
                                    .PutAsync(updatedTenTen);
                                    
                                System.Diagnostics.Debug.WriteLine($"[Firebase] Successfully migrated TenTen {item.Key}");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating topicId: {ex.Message}");
                                // 마이그레이션 실패해도 계속 진행
                            }
                        }
                    }
                    
                    if (isMatch)
                    {
                        tenTens.Add(ConvertFromFirebase(item.Object, item.Key, idCounter++));
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Loaded {tenTens.Count} tentens for topic {topicFirebaseKey}");
                return tenTens;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error loading tentens by topic: {ex.Message}");
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
                return allTenTens.Where(t => t.TopicId == topicId.ToString()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting tentens by topic {topicId}: {ex.Message}");
                return new List<TenTen>();
            }
        }

        public async Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request)
        {
            return await CreateTenTenAsync(userId, request, "personal");
        }

        public async Task<TenTen> CreateTenTenAsync(string userId, CreateTenTenRequest request, string encryptionType)
        {
            try
            {
                // Topic의 Firebase Key 조회
                var topics = await GetTopicsAsync(userId);
                var topic = topics.FirstOrDefault(t => t.Id.ToString() == request.TopicId);
                var topicFirebaseKey = topic?.FirebaseKey ?? request.TopicId.ToString();
                
                string encryptedContent;
                
                // Content 암호화
                if (encryptionType == "shared" && _encryptionService.HasSharedDEK)
                {
                    // 공유 DEK로 암호화
                    encryptedContent = await _encryptionService.EncryptWithSharedDEKAsync(request.Content);
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Encrypted with shared DEK for topic {request.TopicId}");
                }
                else if (encryptionType == "shared" && !_encryptionService.HasSharedDEK)
                {
                    // Shared DEK가 없는 경우 개인 DEK로 암호화 (fallback)
                    encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Shared DEK not available, encrypted with personal DEK for topic {request.TopicId}");
                }
                else
                {
                    // 개인 DEK로 암호화
                    encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Encrypted with personal DEK for topic {request.TopicId}");
                }
                
                var firebaseTenTen = new FirebaseTenTen
                {
                    TopicId = topicFirebaseKey, // Firebase Key 사용
                    Content = encryptedContent,
                    IsEncrypted = true,
                    EncryptionType = encryptionType,
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
                tenTen.EncryptionType = encryptionType;
                System.Diagnostics.Debug.WriteLine($"[Firebase] Created tenten for topic {request.TopicId} with encryption type {encryptionType}");
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
                // 기존 항목의 암호화 타입을 확인하여 일관성 유지
                var current = await GetTenTenByIdAsync(userId, tenTenId);
                var encryptionType = current?.EncryptionType ?? "personal";
                return await UpdateTenTenAsync(userId, tenTenId, request, encryptionType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating tenten {tenTenId}: {ex.Message}");
                throw;
            }
        }

        public async Task<TenTen> UpdateTenTenAsync(string userId, string tenTenId, UpdateTenTenRequest request, string encryptionType)
        {
            try
            {
                // 암호화 방식 결정: 공유 DEK 가용 여부에 따라 분기
                string encryptedContent;
                string typeToSave = encryptionType;

                if (encryptionType == "shared")
                {
                    if (_encryptionService.HasSharedDEK)
                    {
                        encryptedContent = await _encryptionService.EncryptWithSharedDEKAsync(request.Content);
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Updated with shared DEK for {tenTenId}");
                    }
                    else
                    {
                        // 정책 B: 공유 DEK가 없으면 개인 DEK로 저장하고 타입을 personal로 동기화
                        encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                        typeToSave = "personal";
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Shared DEK not available. Updated with personal DEK and set type to personal for {tenTenId}");
                    }
                }
                else
                {
                    encryptedContent = await _encryptionService.EncryptAsync(request.Content);
                }

                var updates = new
                {
                    content = encryptedContent,
                    isEncrypted = true,
                    encryptionType = typeToSave,
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
                    updatedTenTen.EncryptionType = typeToSave;
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

        public async Task<bool> DeleteTenTensByTopicAsync(string userId, string topicFirebaseKey)
        {
            try
            {
                var tenTens = await GetTenTensByTopicAsync(userId, topicFirebaseKey);
                
                foreach (var tenTen in tenTens)
                {
                    if (!string.IsNullOrEmpty(tenTen.FirebaseKey))
                    {
                        await DeleteTenTenAsync(userId, tenTen.FirebaseKey);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] Deleted {tenTens.Count} tentens for topic {topicFirebaseKey}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error deleting tentens by topic {topicFirebaseKey}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Partner Management

        public async Task<FirebaseUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                var users = await _firebaseClient
                    .Child("users")
                    .OnceAsync<FirebaseUser>();

                var user = users.FirstOrDefault(u => u.Object.Email == email);
                return user?.Object;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting user by email {email}: {ex.Message}");
                return null;
            }
        }

        public async Task<(FirebaseUser? user, string? userId)> GetUserWithIdByEmailAsync(string email)
        {
            try
            {
                var users = await _firebaseClient
                    .Child("users")
                    .OnceAsync<FirebaseUser>();

                var user = users.FirstOrDefault(u => u.Object.Email == email);
                return (user?.Object, user?.Key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting user with ID by email {email}: {ex.Message}");
                return (null, null);
            }
        }

        public async Task<FirebaseUser?> GetUserAsync(string userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Getting user by ID: {userId}");
                var user = await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .OnceSingleAsync<FirebaseUser>();

                System.Diagnostics.Debug.WriteLine($"[Firebase] User data retrieved: {user != null}");
                if (user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User email: {user.Email}");
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User display name: {user.DisplayName}");
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User has EncryptedDEK: {!string.IsNullOrEmpty(user.EncryptedDEK)}");
                    System.Diagnostics.Debug.WriteLine($"[Firebase] User has Partner: {user.Partner != null}");
                    
                    if (user.Partner != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Partner ID: {user.Partner.PartnerId}");
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Partner Email: {user.Partner.PartnerEmail}");
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Partner Display Name: {user.Partner.PartnerDisplayName}");
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Partner Connected At: {user.Partner.ConnectedAt}");
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Partner has EncryptedSharedDEK: {user.Partner.EncryptedSharedDEK != null}");
                        
                        if (user.Partner.EncryptedSharedDEK != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] EncryptedSharedDEK type: {user.Partner.EncryptedSharedDEK.GetType().Name}");
                            var dekValue = user.Partner.GetEncryptedSharedDEKValue();
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Extracted DEK value exists: {!string.IsNullOrEmpty(dekValue)}");
                        }
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting user by ID {userId}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Inner exception: {ex.InnerException.Message}");
                }
                return null;
            }
        }

        public async Task UpdatePartnerInfoAsync(string userId, PartnerInfo partnerInfo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Updating partner info for user: {userId}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Partner ID: {partnerInfo.PartnerId}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Partner Email: {partnerInfo.PartnerEmail}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Partner Display Name: {partnerInfo.PartnerDisplayName}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Connected At: {partnerInfo.ConnectedAt}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Encrypted Shared DEK: {partnerInfo.EncryptedSharedDEK != null}");

                await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .Child("partner")
                    .PutAsync(partnerInfo);

                System.Diagnostics.Debug.WriteLine($"[Firebase] Successfully updated partner info for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating partner info: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Firebase] Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Firebase] Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task RemovePartnerInfoAsync(string userId)
        {
            try
            {
                await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .Child("partner")
                    .DeleteAsync();

                System.Diagnostics.Debug.WriteLine($"[Firebase] Removed partner info for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error removing partner info: {ex.Message}");
                throw;
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
                TopicId = firebaseTenTen.TopicId ?? string.Empty,
                Content = firebaseTenTen.Content ?? string.Empty,
                CreatedAt = ParseDate(firebaseTenTen.CreatedAt, DateTime.Now),
                UpdatedAt = string.IsNullOrEmpty(firebaseTenTen.UpdatedAt) ? null : ParseDate(firebaseTenTen.UpdatedAt, null),
                UserId = 1,
                UserName = "현재 사용자",
                TopicSubject = "",
                IsReadByPartner = false,
                IsEncrypted = firebaseTenTen.IsEncrypted,
                EncryptionType = firebaseTenTen.EncryptionType ?? "personal"
            };
        }

        #endregion

        #region Real-time Listeners

        public async Task<IDisposable> ObservePartnerTopicsAsync(string partnerUserId, Action<List<Topic>> onTopicsChanged)
        {
            try
            {
                var observable = _firebaseClient
                    .Child("topics")
                    .Child(partnerUserId)
                    .AsObservable<Dictionary<string, FirebaseTopic>>();

                var subscription = observable.Subscribe(
                    onNext: async (firebaseEvent) =>
                    {
                        try
                        {
                            var topics = new List<Topic>();
                            if (firebaseEvent?.Object != null)
                            {
                                foreach (var kvp in firebaseEvent.Object)
                                {
                                    if (kvp.Value.IsActive)
                                    {
                                        var topic = ConvertFromFirebase(kvp.Value, kvp.Key, 0);
                                        topics.Add(topic);
                                    }
                                }
                            }
                            onTopicsChanged(topics);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Error processing partner topics update: {ex.Message}");
                        }
                    },
                    onError: ex =>
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Error observing partner topics: {ex.Message}");
                    });

                System.Diagnostics.Debug.WriteLine($"[Firebase] Started observing partner topics for user: {partnerUserId}");
                return subscription;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error starting partner topics observer: {ex.Message}");
                throw;
            }
        }

        public async Task<IDisposable> ObservePartnerTenTensAsync(string partnerUserId, string topicFirebaseKey, Action<List<TenTen>> onTenTensChanged)
        {
            try
            {
                var observable = _firebaseClient
                    .Child("tentens")
                    .Child(partnerUserId)
                    .AsObservable<Dictionary<string, FirebaseTenTen>>();

                var subscription = observable.Subscribe(
                    onNext: async (firebaseEvent) =>
                    {
                        try
                        {
                            var tenTens = new List<TenTen>();
                            if (firebaseEvent?.Object != null)
                            {
                                foreach (var kvp in firebaseEvent.Object)
                                {
                                    if (kvp.Value.TopicId == topicFirebaseKey)
                                    {
                                        var tenTen = ConvertFromFirebase(kvp.Value, kvp.Key, 0);
                                        tenTens.Add(tenTen);
                                    }
                                }
                            }
                            onTenTensChanged(tenTens);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Error processing partner tentens update: {ex.Message}");
                        }
                    },
                    onError: ex =>
                    {
                        System.Diagnostics.Debug.WriteLine($"[Firebase] Error observing partner tentens: {ex.Message}");
                    });

                System.Diagnostics.Debug.WriteLine($"[Firebase] Started observing partner tentens for topic: {topicFirebaseKey}");
                return subscription;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error starting partner tentens observer: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Pending Shared DEK Management

        public async Task SavePendingSharedDEKAsync(string userId, byte[] sharedDek, string inviterUserId)
        {
            try
            {
                var pendingData = new
                {
                    sharedDEK = Convert.ToBase64String(sharedDek),
                    inviterUserId = inviterUserId,
                    createdAt = DateTime.Now.ToString("o")
                };

                await _firebaseClient
                    .Child("pending_shared_deks")
                    .Child(userId)
                    .PutAsync(pendingData);

                System.Diagnostics.Debug.WriteLine($"[Firebase] Saved pending shared DEK for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error saving pending shared DEK: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]?> GetPendingSharedDEKAsync(string userId)
        {
            try
            {
                var pendingData = await _firebaseClient
                    .Child("pending_shared_deks")
                    .Child(userId)
                    .OnceSingleAsync<dynamic>();

                if (pendingData?.sharedDEK != null)
                {
                    string sharedDEKString = pendingData.sharedDEK?.ToString();
                    if (!string.IsNullOrEmpty(sharedDEKString))
                    {
                        try
                        {
                            return Convert.FromBase64String(sharedDEKString);
                        }
                        catch (FormatException ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Firebase] Invalid Base64 format for pending shared DEK: {ex.Message}");
                            return null;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[Firebase] No pending shared DEK found for user: {userId}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error getting pending shared DEK: {ex.Message}");
                return null;
            }
        }

        public async Task DeletePendingSharedDEKAsync(string userId)
        {
            try
            {
                await _firebaseClient
                    .Child("pending_shared_deks")
                    .Child(userId)
                    .DeleteAsync();

                System.Diagnostics.Debug.WriteLine($"[Firebase] Deleted pending shared DEK for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error deleting pending shared DEK: {ex.Message}");
                throw;
            }
        }

        public async Task UpdatePartnerSharedDEKAsync(string userId, string encryptedSharedDEK)
        {
            try
            {
                // JSON 객체로 래핑해서 저장 (Firebase 호환성)
                var dekData = new
                {
                    value = encryptedSharedDEK,
                    timestamp = DateTime.Now.ToString("o")
                };
                
                await _firebaseClient
                    .Child("users")
                    .Child(userId)
                    .Child("partner")
                    .Child("encryptedSharedDEK")
                    .PutAsync(dekData);

                System.Diagnostics.Debug.WriteLine($"[Firebase] Updated partner shared DEK for user: {userId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Firebase] Error updating partner shared DEK: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}

