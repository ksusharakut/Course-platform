namespace Course_platform.Models
{
    public class UserCourseProgressEntity
    {
        public int ProgressId { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public string State { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } 

        public virtual CourseEntity? Course { get; set; }

        public virtual UserEntity? User { get; set; }
    }
}
