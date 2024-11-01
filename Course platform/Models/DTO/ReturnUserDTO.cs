namespace Course_platform.Models.DTO
{
    public class ReturnUserDTO
    {
        public int UserId { get; set; }

        public string Nickname { get; set; }

        public DateOnly DateBirth { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } 

        public int AccountBalance { get; set; }
    }
}
