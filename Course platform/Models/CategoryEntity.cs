namespace Course_platform.Models
{
    public class CategoryEntity
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();
    }
}
