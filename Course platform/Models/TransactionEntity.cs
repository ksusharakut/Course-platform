using System.Text.Json.Serialization;

namespace Course_platform.Models
{
    public class TransactionEntity
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }

        public int Amount { get; set; }

        public string? TransactionType { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public int? CourseId { get; set; }

        public int PlatformFee { get; set; }

        public virtual CourseEntity? Course { get; set; }

        [JsonIgnore]
        public virtual UserEntity? User { get; set; }
    }
}
