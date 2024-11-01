namespace Course_platform.Models
{
    public class PasswordResetTokenEntity
    {
        public int ResetTokenId { get; set; }
        public string Token { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Email { get; set; }
    }
}
