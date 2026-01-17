using Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auction.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(u => u.BlockedBalance)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}