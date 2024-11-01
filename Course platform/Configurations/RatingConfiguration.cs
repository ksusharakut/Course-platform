using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<RatingEntity>
    {
        public void Configure(EntityTypeBuilder<RatingEntity> builder)
        {
            builder.ToTable("ratings");

            builder.HasKey(r => r.RatingId);

            builder.Property(r => r.RatingId)
                    .ValueGeneratedOnAdd();

            builder.HasOne(r => r.User)
                   .WithMany(u => u.Ratings)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Course)
                   .WithMany(c => c.Ratings)
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("NOW()");
        }
    }
}
