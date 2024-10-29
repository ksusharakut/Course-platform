namespace Course_platform.Models.DTO
{
    public class RegistrateUserDTO
    {
        public string Nickname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateOnly? DateBirth { get; set; }
    }
}
