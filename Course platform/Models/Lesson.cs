using System;
using System.Collections.Generic;
using Course_platform.Models;

namespace Course_platform;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int? ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public bool? IsComplited { get; set; }

    public virtual Module? Module { get; set; }
}
