namespace Course_platform.Models.DTO
{
    public class RegistrateUserDTO
    {
        public string Nickname { get; set; } 

        public string Email { get; set; } 

        public string Password { get; set; } 

        public DateOnly DateBirth { get; set; }
    }
}
