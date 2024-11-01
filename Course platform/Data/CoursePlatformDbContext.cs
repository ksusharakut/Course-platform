using System;
using System.Collections.Generic;
using System.Reflection;
using Course_platform.Configurations;
using Course_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Course_platform;

public class CoursePlatformDbContext : DbContext
{
    public CoursePlatformDbContext(DbContextOptions<CoursePlatformDbContext> options)
        : base(options){
    }

    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<UserCourseProgressEntity> UserCourseProgresses { get; set; }
    public virtual DbSet<PasswordResetTokenEntity> PasswordResetTokens { get; set; }
    public virtual DbSet<CourseEntity> Courses { get; set; }
    public virtual DbSet<UnitEntity> Units { get; set; }
    public virtual DbSet<LessonEntity> Lessons { get; set; }
    public virtual DbSet<RatingEntity> Ratings { get; set; }
    public virtual DbSet<CategoryEntity> Categories { get; set; }
    public virtual DbSet<TransactionEntity> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfigutation());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new UnitConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
        modelBuilder.ApplyConfiguration(new RatingConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new UserCourseProgressConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
