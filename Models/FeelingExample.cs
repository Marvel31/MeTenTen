namespace MeTenTenMaui.Models
{
    public class FeelingExample
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty; // 기쁨, 두려움, 분노, 슬픔
        public string SubCategory { get; set; } = string.Empty; // 가벼운, 개운한 등
        public string Description { get; set; } = string.Empty; // 예시 설명
        public bool IsDefault { get; set; } // 기본 예시 vs 사용자 추가
        public DateTime CreatedAt { get; set; }
    }
}


