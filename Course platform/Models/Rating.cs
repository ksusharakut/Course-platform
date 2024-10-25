using System;
using System.Collections.Generic;

namespace Course_platform.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public int? Rating1 { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
}
