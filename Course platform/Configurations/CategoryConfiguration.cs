using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.CategoryId)
                    .ValueGeneratedOnAdd();

            builder.HasIndex(c => c.CategoryName)
                   .IsUnique();

            builder.HasMany(c => c.Courses)
                   .WithMany(ct => ct.Categories)
                   .UsingEntity(j => j.ToTable("course_categories"));
        }
    }
}
