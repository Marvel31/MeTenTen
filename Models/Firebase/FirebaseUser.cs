namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseUser
    {
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? EncryptedDEK { get; set; } // 비밀번호로 암호화된 DEK
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedAt { get; set; }
    }
}

