using System.Reflection;

namespace Course_platform.Models
{
    public class LessonEntity
    {
        public int LessonId { get; set; }

        public int UnitId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public virtual UnitEntity? Unit { get; set; }
    }
}
