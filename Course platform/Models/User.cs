using System;
using System.Collections.Generic;
using Course_platform.Models;

namespace Course_platform;

public partial class User
{
    public int UserId { get; set; }

    public string Nickname { get; set; } = null!;

    public DateOnly? DateBirth { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string Role { get; set; } = null!;

    public bool VerifiedDegree { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public string? AvatarUrl { get; set; }

    public string? AvatarThumbnailUrl { get; set; }

    public int AccountBalance { get; set; }

    public virtual ICollection<Chat> ChatUser1s { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatUser2s { get; set; } = new List<Chat>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserCourseProgress> UserCourseProgresses { get; set; } = new List<UserCourseProgress>();

    public virtual ICollection<User> FollowedUsers { get; set; } = new List<User>();

    public virtual ICollection<User> FollowingUsers { get; set; } = new List<User>();
}
