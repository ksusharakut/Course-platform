namespace Course_platform.Models.DTO
{
    public class PasswordRecoveryDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; } 

    }
}
