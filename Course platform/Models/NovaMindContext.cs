using System;
using System.Collections.Generic;
using Course_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace Course_platform;

public partial class NovaMindContext : DbContext
{
    public NovaMindContext()
    {
    }

    public NovaMindContext(DbContextOptions<NovaMindContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCourseProgress> UserCourseProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("nextval('category_category_id_seq'::regclass)")
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("chat_pkey");

            entity.ToTable("chats");

            entity.HasIndex(e => new { e.User1Id, e.User2Id }, "chat_user1_id_user2_id_key").IsUnique();

            entity.Property(e => e.ChatId)
                .HasDefaultValueSql("nextval('chat_chat_id_seq'::regclass)")
                .HasColumnName("chat_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");

            entity.HasOne(d => d.User1).WithMany(p => p.ChatUser1s)
                .HasForeignKey(d => d.User1Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chat_user1_id_fkey");

            entity.HasOne(d => d.User2).WithMany(p => p.ChatUser2s)
                .HasForeignKey(d => d.User2Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chat_user2_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasDefaultValue(0)
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Courses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("courses_user_id_fkey");

            entity.HasMany(d => d.Categories).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCategoryMap",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("course_category_map_category_id_fkey"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("course_category_map_course_id_fkey"),
                    j =>
                    {
                        j.HasKey("CourseId", "CategoryId").HasName("course_category_map_pkey");
                        j.ToTable("course_category_map");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("lessons_pkey");

            entity.ToTable("lessons");

            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IsComplited)
                .HasDefaultValue(false)
                .HasColumnName("is_complited");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("lessons_module_id_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("message_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId)
                .HasDefaultValueSql("nextval('message_message_id_seq'::regclass)")
                .HasColumnName("message_id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.Read)
                .HasDefaultValue(false)
                .HasColumnName("read");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("sent_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("message_chat_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("message_user_id_fkey");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("modules_pkey");

            entity.ToTable("modules");

            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.IsComplited)
                .HasDefaultValue(false)
                .HasColumnName("is_complited");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("modules_course_id_fkey");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("rating_pkey");

            entity.ToTable("ratings");

            entity.Property(e => e.RatingId)
                .HasDefaultValueSql("nextval('rating_rating_id_seq'::regclass)")
                .HasColumnName("rating_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating1).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rating_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rating_user_id_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.PlatformFee).HasColumnName("platform_fee");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .HasColumnName("transaction_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactions_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactions_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "unique_email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AccountBalance)
                .HasDefaultValue(0)
                .HasColumnName("account_balance");
            entity.Property(e => e.AvatarThumbnailUrl).HasColumnName("avatar_thumbnail_url");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.DateBirth).HasColumnName("date_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(225)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("regular_user")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("time with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VerifiedDegree)
                .HasDefaultValue(false)
                .HasColumnName("verified_degree");

            entity.HasMany(d => d.FollowedUsers).WithMany(p => p.FollowingUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "Follower",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FollowedUserId")
                        .HasConstraintName("followers_followed_user_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FollowingUserId")
                        .HasConstraintName("followers_following_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("FollowingUserId", "FollowedUserId").HasName("followers_pkey");
                        j.ToTable("followers");
                        j.IndexerProperty<int>("FollowingUserId").HasColumnName("following_user_id");
                        j.IndexerProperty<int>("FollowedUserId").HasColumnName("followed_user_id");
                    });

            entity.HasMany(d => d.FollowingUsers).WithMany(p => p.FollowedUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "Follower",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("FollowingUserId")
                        .HasConstraintName("followers_following_user_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("FollowedUserId")
                        .HasConstraintName("followers_followed_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("FollowingUserId", "FollowedUserId").HasName("followers_pkey");
                        j.ToTable("followers");
                        j.IndexerProperty<int>("FollowingUserId").HasColumnName("following_user_id");
                        j.IndexerProperty<int>("FollowedUserId").HasColumnName("followed_user_id");
                    });
        });

        modelBuilder.Entity<UserCourseProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("usercourseprogress_pkey");

            entity.ToTable("user_course_progress");

            entity.HasIndex(e => new { e.UserId, e.CourseId }, "usercourseprogress_user_id_course_id_key").IsUnique();

            entity.Property(e => e.ProgressId)
                .HasDefaultValueSql("nextval('usercourseprogress_progress_id_seq'::regclass)")
                .HasColumnName("progress_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasDefaultValueSql("'not started'::character varying")
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.UserCourseProgresses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usercourseprogress_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserCourseProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usercourseprogress_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
