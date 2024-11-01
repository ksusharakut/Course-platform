using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<LessonEntity>
    {
        public void Configure(EntityTypeBuilder<LessonEntity> builder)
        {
            builder.ToTable("lessons");

            builder.HasKey(l => l.LessonId);

            builder.Property(l => l.LessonId)
                .ValueGeneratedOnAdd();

            builder.HasOne(l => l.Unit)
                   .WithMany(m => m.Lessons)
                   .HasForeignKey(l => l.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
