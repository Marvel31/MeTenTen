using MeTenTenMaui.Models;
using MeTenTenMaui.Models.Firebase;
using System.Security.Cryptography;
using System.Text;

namespace MeTenTenMaui.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IFirebaseDataService _firebaseDataService;
        private readonly IEncryptionService _encryptionService;
        private readonly IAuthService _authService;
        private string? _currentUserId;
        private string? _currentUserEmail;

        public PartnerService(IFirebaseDataService firebaseDataService, IEncryptionService encryptionService, IAuthService authService)
        {
            _firebaseDataService = firebaseDataService;
            _encryptionService = encryptionService;
            _authService = authService;
        }

        private void InitializeCurrentUser()
        {
            // 매번 최신 사용자 정보 가져오기 (캐시 사용 안 함)
            _currentUserId = _authService.CurrentUserId;
            _currentUserEmail = _authService.CurrentUserEmail;
            
            System.Diagnostics.Debug.WriteLine($"[PartnerService] InitializeCurrentUser - UserId: {_currentUserId}, Email: {_currentUserEmail}");
        }

        public async Task<(bool success, string message)> InvitePartnerAsync(string partnerEmail, string myPassword)
        {
            try
            {
                InitializeCurrentUser();
                if (_currentUserId == null || _currentUserEmail == null)
                {
                    return (false, "로그인이 필요합니다.");
                }

                // 자신의 이메일인지 확인
                if (partnerEmail.Equals(_currentUserEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "자신의 이메일은 입력할 수 없습니다.");
                }

                // 배우자 이메일로 사용자 검색 (사용자 ID 포함)
                var (partnerUser, partnerUserId) = await _firebaseDataService.GetUserWithIdByEmailAsync(partnerEmail);
                if (partnerUser == null || partnerUserId == null)
                {
                    return (false, "해당 이메일로 가입된 사용자를 찾을 수 없습니다.");
                }

                // 이미 배우자가 있는지 확인
                var currentUser = await _firebaseDataService.GetUserByEmailAsync(_currentUserEmail);
                if (currentUser?.Partner != null)
                {
                    return (false, "이미 배우자와 연결되어 있습니다.");
                }

                // 배우자도 이미 연결되어 있는지 확인
                if (partnerUser.Partner != null)
                {
                    return (false, "해당 사용자는 이미 다른 배우자와 연결되어 있습니다.");
                }

                // 공유 DEK 생성
                var sharedDek = await _encryptionService.GenerateSharedDEKAsync();
                
                // 내 비밀번호로 공유 DEK 암호화
                var myEncryptedSharedDEK = await _encryptionService.EncryptDEKAsync(
                    sharedDek, _currentUserEmail, myPassword);

                // 내 계정에 배우자 정보 설정
                var currentUserPartnerInfo = new PartnerInfo
                {
                    PartnerId = partnerUserId, // 배우자의 실제 사용자 ID
                    PartnerEmail = partnerEmail,
                    PartnerDisplayName = partnerUser.DisplayName ?? partnerEmail,
                    ConnectedAt = DateTime.Now.ToString("o"),
                    EncryptedSharedDEK = new 
                    {
                        value = myEncryptedSharedDEK,
                        timestamp = DateTime.Now.ToString("o")
                    }
                };

                // Firebase에 내 배우자 정보 저장
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 1: Saving User1 partner info for user: {_currentUserId}");
                await _firebaseDataService.UpdatePartnerInfoAsync(_currentUserId, currentUserPartnerInfo);
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 1 Complete: User1 partner info saved");
                
                // User2(배우자)의 partner 정보도 설정
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 2: Creating User2 partner info for user: {partnerUserId}");
                var partnerUserPartnerInfo = new PartnerInfo
                {
                    PartnerId = _currentUserId,
                    PartnerEmail = _currentUserEmail,
                    PartnerDisplayName = currentUser.DisplayName ?? _currentUserEmail,
                    ConnectedAt = DateTime.Now.ToString("o"),
                    EncryptedSharedDEK = "" // 로그인 시 pending DEK로 채워짐
                };
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 2: Saving User2 partner info");
                await _firebaseDataService.UpdatePartnerInfoAsync(partnerUserId, partnerUserPartnerInfo);
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 2 Complete: User2 partner info saved");
                
                // 배우자를 위한 pending shared DEK 저장 (평문)
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 3: Saving pending shared DEK for user: {partnerUserId}");
                await _firebaseDataService.SavePendingSharedDEKAsync(partnerUserId, sharedDek, _currentUserId);
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Step 3 Complete: Pending shared DEK saved");

                return (true, $"배우자와 연결되었습니다: {partnerUser.DisplayName ?? partnerEmail}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error inviting partner: {ex.Message}");
                return (false, "배우자 초대 중 오류가 발생했습니다.");
            }
        }


        public async Task<(bool success, string message)> DisconnectPartnerAsync()
        {
            try
            {
                InitializeCurrentUser();
                if (_currentUserId == null)
                {
                    return (false, "로그인이 필요합니다.");
                }

                // GetUserAsync를 사용하여 JSON 파싱 오류 방지
                var currentUser = await _firebaseDataService.GetUserAsync(_currentUserId);
                if (currentUser?.Partner == null)
                {
                    return (false, "연결된 배우자가 없습니다.");
                }

                var partnerId = currentUser.Partner.PartnerId;
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Disconnecting partner: {partnerId}");

                // 양쪽 사용자의 배우자 정보 제거
                await _firebaseDataService.RemovePartnerInfoAsync(_currentUserId);
                await _firebaseDataService.RemovePartnerInfoAsync(partnerId);

                // 공유 DEK 메모리에서 제거
                _encryptionService.ClearSharedDEK();

                System.Diagnostics.Debug.WriteLine($"[PartnerService] Partner disconnected successfully");
                return (true, "배우자 연결이 해제되었습니다.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error disconnecting partner: {ex.Message}");
                return (false, "배우자 연결 해제 중 오류가 발생했습니다.");
            }
        }

        public async Task<PartnerInfo?> GetPartnerInfoAsync()
        {
            try
            {
                InitializeCurrentUser();
                if (_currentUserId == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[PartnerService] CurrentUserId is null");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"[PartnerService] Getting partner info for user: {_currentUserId}");
                var currentUser = await _firebaseDataService.GetUserAsync(_currentUserId);
                
                if (currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[PartnerService] User data not found for ID: {_currentUserId}");
                    return null;
                }

                var partnerInfo = currentUser.Partner;
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Partner info result: {partnerInfo != null}");
                
                if (partnerInfo != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[PartnerService] Partner email: {partnerInfo.PartnerEmail}, ID: {partnerInfo.PartnerId}");
                }

                return partnerInfo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error getting partner info: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TenTen>> GetPartnerTopicTenTensAsync(string topicFirebaseKey)
        {
            try
            {
                InitializeCurrentUser();
                if (_currentUserId == null)
                {
                    return new List<TenTen>();
                }

                var partnerInfo = await GetPartnerInfoAsync();
                if (partnerInfo?.PartnerId == null || string.IsNullOrEmpty(partnerInfo.GetEncryptedSharedDEKValue()))
                {
                    return new List<TenTen>();
                }

                // 공유 DEK가 이미 메모리에 로드되어 있는지 확인
                if (!_encryptionService.HasSharedDEK)
                {
                    System.Diagnostics.Debug.WriteLine($"[PartnerService] Shared DEK not available in memory");
                    return new List<TenTen>();
                }

                // 배우자의 TenTens 조회
                var tenTens = await _firebaseDataService.GetTenTensByTopicAsync(partnerInfo.PartnerId, topicFirebaseKey);

                // 암호화 타입에 따라 복호화
                foreach (var tenTen in tenTens)
                {
                    System.Diagnostics.Debug.WriteLine($"[PartnerService] Processing TenTen {tenTen.Id}, EncryptionType: {tenTen.EncryptionType}, IsEncrypted: {tenTen.IsEncrypted}");
                    
                    if (tenTen.IsEncrypted && !string.IsNullOrEmpty(tenTen.Content))
                    {
                        if (tenTen.EncryptionType == "shared")
                        {
                            try
                            {
                                tenTen.Content = await _encryptionService.DecryptWithSharedDEKAsync(tenTen.Content);
                                System.Diagnostics.Debug.WriteLine($"[PartnerService] Successfully decrypted shared TenTen {tenTen.Id}");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error decrypting shared TenTen {tenTen.Id}: {ex.Message}");
                                tenTen.Content = "[복호화 실패]";
                            }
                        }
                        else
                        {
                            // personal 타입이거나 shared 타입이지만 개인 DEK로 암호화된 경우
                            // 먼저 공유 DEK로 복호화 시도
                            try
                            {
                                tenTen.Content = await _encryptionService.DecryptWithSharedDEKAsync(tenTen.Content);
                                System.Diagnostics.Debug.WriteLine($"[PartnerService] Successfully decrypted TenTen {tenTen.Id} with shared DEK");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"[PartnerService] Failed to decrypt TenTen {tenTen.Id} with shared DEK: {ex.Message}");
                                tenTen.Content = "[개인 데이터 - 열람 불가]";
                            }
                        }
                    }
                }

                return tenTens;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error getting partner topic TenTens: {ex.Message}");
                return new List<TenTen>();
            }
        }

        public async Task<List<Topic>> GetPartnerCompletedTopicsAsync()
        {
            try
            {
                InitializeCurrentUser();
                if (_currentUserId == null)
                {
                    return new List<Topic>();
                }

                var partnerInfo = await GetPartnerInfoAsync();
                if (partnerInfo?.PartnerId == null)
                {
                    return new List<Topic>();
                }

                // 배우자의 모든 Topic 조회
                var allTopics = await _firebaseDataService.GetAllTopicsAsync(partnerInfo.PartnerId);
                
                // 완료된 Topic만 필터링 (TenTen이 있고, IsActive=true이며, shared TenTen이 있는 Topic)
                var completedTopics = new List<Topic>();
                foreach (var topic in allTopics)
                {
                    // IsActive=false인 삭제된 주제는 제외
                    if (!topic.IsActive)
                    {
                        continue;
                    }

                    var tenTens = await _firebaseDataService.GetTenTensByTopicAsync(partnerInfo.PartnerId, topic.FirebaseKey);
                    if (tenTens.Any())
                    {
                        // TenTen이 하나라도 있으면 완료된 Topic으로 간주
                        // TODO: 향후 shared/personal 구분 로직 추가 필요
                        completedTopics.Add(topic);
                    }
                }

                return completedTopics.OrderByDescending(t => t.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerService] Error getting partner completed topics: {ex.Message}");
                return new List<Topic>();
            }
        }


    }
}

