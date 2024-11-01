namespace Course_platform.Models
{
    public class UnitEntity
    {
        public int UnitId { get; set; }

        public int CourseId { get; set; }

        public int OrderIndex { get; set; }

        public string Title { get; set; } 

        public virtual CourseEntity? Course { get; set; }

        public virtual ICollection<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
    }
}
