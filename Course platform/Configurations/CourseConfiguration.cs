using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<CourseEntity>
    {
        public void Configure(EntityTypeBuilder<CourseEntity> builder)
        {
            builder.ToTable("courses");

            builder.HasKey(c => c.CourseId);

            builder.Property(c => c.CourseId)
                    .ValueGeneratedOnAdd();

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Courses)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            builder.Property(c => c.Price)
                  .IsRequired()
                  .HasDefaultValue(0);

            builder.HasMany(c => c.Units)
                  .WithOne(m => m.Course)
                  .HasForeignKey(m => m.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Ratings)
                   .WithOne(r => r.Course)
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Transactions)
                   .WithOne(t => t.Course)
                   .HasForeignKey(t => t.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.UserCourseProgresses)
                   .WithOne(ucp => ucp.Course)
                   .HasForeignKey(ucp => ucp.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Categories)
                   .WithMany(ct => ct.Courses)
                   .UsingEntity(j => j.ToTable("CourseCategories"));
        }
    }
}
