using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<UnitEntity>
    {
        public void Configure(EntityTypeBuilder<UnitEntity> builder)
        {
            builder.ToTable("units");

            builder.HasKey(m => m.UnitId);

            builder.Property(m => m.UnitId)
                  .ValueGeneratedOnAdd();

            builder.HasOne(m => m.Course)
                  .WithMany(c => c.Units)
                  .HasForeignKey(m => m.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Lessons)
                   .WithOne(l => l.Unit)
                   .HasForeignKey(l => l.UnitId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
