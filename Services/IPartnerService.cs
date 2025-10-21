using MeTenTenMaui.Models;
using MeTenTenMaui.Models.Firebase;

namespace MeTenTenMaui.Services
{
    public interface IPartnerService
    {
        // 배우자 초대 (공유 DEK 생성 포함)
        Task<(bool success, string message)> InvitePartnerAsync(string partnerEmail, string myPassword);
        
        // 배우자 연결 해제
        Task<(bool success, string message)> DisconnectPartnerAsync();
        
        // 배우자 정보 조회
        Task<PartnerInfo?> GetPartnerInfoAsync();
        
        // 배우자가 작성 완료한 Topic 목록 조회
        Task<List<Topic>> GetPartnerCompletedTopicsAsync();
        
        // 배우자의 특정 Topic의 TenTens 조회 (공유 DEK 사용)
        Task<List<TenTen>> GetPartnerTopicTenTensAsync(string topicFirebaseKey);
    }
}
