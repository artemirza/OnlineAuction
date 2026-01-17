using Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auction.Infrastructure.Data.Configurations;

public sealed class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.AuctionId)
            .IsRequired();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(b => b.AuctionId);

        builder.HasOne(b => b.Auction)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bids)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}