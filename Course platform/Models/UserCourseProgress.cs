using System;
using System.Collections.Generic;

namespace Course_platform.Models;

public partial class UserCourseProgress
{
    public int ProgressId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public string? State { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
}
