using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class UserCourseProgressConfiguration : IEntityTypeConfiguration<UserCourseProgressEntity>
    {
        public void Configure(EntityTypeBuilder<UserCourseProgressEntity> builder)
        {
            builder.ToTable("userc_course_progress");

            builder.HasKey(p => p.ProgressId);

            builder.Property(p => p.ProgressId)
                    .ValueGeneratedOnAdd();

            builder.Property(p => p.State)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("in_progress");

            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("NOW()");

            builder.HasOne(p => p.Course)
                   .WithMany(c => c.UserCourseProgresses)
                   .HasForeignKey(p => p.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.UserCourseProgresses)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
