using System;
using System.Collections.Generic;

namespace Course_platform.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public int? CourseId { get; set; }

    public int OrderIndex { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsComplited { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
