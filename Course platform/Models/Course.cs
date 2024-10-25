using System;
using System.Collections.Generic;
using Course_platform.Models;

namespace Course_platform;

public partial class Course
{
    public int CourseId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Price { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }

    public virtual ICollection<UserCourseProgress> UserCourseProgresses { get; set; } = new List<UserCourseProgress>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
