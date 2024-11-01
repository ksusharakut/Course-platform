namespace Course_platform.Models
{
    public class UserEntity
    {
        public int UserId { get; set; }

        public string Nickname { get; set; } 

        public DateOnly DateBirth { get; set; }

        public string Email { get; set; } 

        public string PasswordHash { get; set; }

        public string Role { get; set; } 

        public DateTimeOffset UpdatedAt { get; set; } 

        public string? AvatarUrl { get; set; }

        public string? AvatarThumbnailUrl { get; set; }

        public int AccountBalance { get; set; } 

        public virtual ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();

        public virtual ICollection<RatingEntity> Ratings { get; set; } = new List<RatingEntity>();

        public virtual ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();

        public virtual ICollection<UserCourseProgressEntity> UserCourseProgresses { get; set; } = new List<UserCourseProgressEntity>();

        public virtual ICollection<UserEntity> FollowedUsers { get; set; } = new List<UserEntity>();

        public virtual ICollection<UserEntity> FollowingUsers { get; set; } = new List<UserEntity>();
    }
}
