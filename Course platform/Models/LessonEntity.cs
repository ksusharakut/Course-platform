using System.Reflection;
using System.Text.Json.Serialization;

namespace Course_platform.Models
{
    public class LessonEntity
    {
        public int LessonId { get; set; }

        public int UnitId { get; set; }

        public string Title { get; set; }

        // Хранит путь к файлу, где содержится текст урока
        public string FilePath { get; set; }

        // Индекс порядка урока в юните
        public int OrderIndex { get; set; }

        [JsonIgnore]
        public virtual UnitEntity? Unit { get; set; }
    }
}
