    using System.Text.Json.Serialization;

    namespace Course_platform.Models.DTO
    {
        public class ReturnCourseDTO
        {
            public int CourseId { get; set; }

            public int UserId { get; set; }

            public string Title { get; set; }

            public string? Description { get; set; }
            public int Price { get; set; }
            public List<int> CategoryIds { get; set; }

            public bool IsPublished { get; set; }

        }
    }
