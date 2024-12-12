using System.Text.Json.Serialization;

namespace Course_platform.Models
{
    public class CourseEntity
    {
        public int CourseId { get; set; }

        public int UserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } 

        public string Title { get; set; } 

        public string? Description { get; set; }

        public int Price { get; set; }
        public bool IsPublished { get; set; } = false;

        public virtual ICollection<UnitEntity> Units { get; set; } = new List<UnitEntity>();

        public virtual ICollection<RatingEntity> Ratings { get; set; } = new List<RatingEntity>();

        public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();

        public virtual UserEntity? User { get; set; }

        public virtual ICollection<UserCourseProgressEntity> UserCourseProgresses { get; set; } = new List<UserCourseProgressEntity>();

        [JsonIgnore]
        public virtual ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
    }
}
