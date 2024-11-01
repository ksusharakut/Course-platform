using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.TransactionId)
                    .ValueGeneratedOnAdd();

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Transactions)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Course)
                   .WithMany(c => c.Transactions)
                   .HasForeignKey(t => t.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("NOW()");
        }
    }
}
