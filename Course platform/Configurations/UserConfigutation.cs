using Course_platform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Course_platform.Configurations
{
    public class UserConfigutation : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .ValueGeneratedOnAdd();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Role)
                .HasDefaultValue("regularUser");

            builder.Property(u => u.AccountBalance)
               .HasDefaultValue(0);

            builder.Property(u => u.UpdatedAt)
                .HasDefaultValueSql("NOW()");

            builder.HasMany(u => u.FollowedUsers)
                .WithMany(u => u.FollowingUsers)
                .UsingEntity(j => j.ToTable("subscriptions"));
        }
    }
}
