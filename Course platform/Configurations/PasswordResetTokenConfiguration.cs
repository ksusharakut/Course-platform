using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course_platform.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetTokenEntity>
    {
        public void Configure(EntityTypeBuilder<PasswordResetTokenEntity> builder)
        {
            builder.ToTable("password_reset_tokens");

            builder.HasKey(t => t.ResetTokenId);

            builder.Property(t => t.ResetTokenId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Token)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("NOW()");
        }
    }
}
