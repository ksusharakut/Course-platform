namespace Course_platform.Models.DTO
{
    public class CreateLessonDTO
    {
        public int UnitId { get; set; } // ID юнита, к которому принадлежит урок
        public string Title { get; set; } // Название урока
        public string Content { get; set; } // Содержимое урока в формате HTML
    }
}
