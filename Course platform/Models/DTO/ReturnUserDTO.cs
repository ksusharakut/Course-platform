namespace Course_platform.Models.DTO
{
    public class ReturnUserDTO
    {
        public int UserId { get; set; }

        public string Nickname { get; set; } = null!;

        public DateOnly? DateBirth { get; set; }

        public string Email { get; set; } = null!;


        public string Role { get; set; } = null!;

        public bool VerifiedDegree { get; set; }

        public int AccountBalance { get; set; }
    }
}
