namespace Course_platform.Models
{
    public class RatingEntity
    {
        public int RatingId { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public int UserRating { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public virtual CourseEntity Course { get; set; }

        public virtual UserEntity User { get; set; }
    }
}
