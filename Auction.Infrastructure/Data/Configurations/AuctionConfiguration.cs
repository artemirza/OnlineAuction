using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auction.Infrastructure.Data.Configurations;

public sealed class AuctionConfiguration: IEntityTypeConfiguration<Domain.Entities.Auction>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Auction> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.StartingPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.CurrentPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.BuyNowPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.StartTime)
            .IsRequired();

        builder.Property(a => a.EndTime)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.CreatorId)
            .IsRequired();

        builder.Property(a => a.WinnerId);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.EndTime);

        builder.HasOne(a => a.Creator)
            .WithMany(u => u.CreatedAuctions)
            .HasForeignKey(a => a.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Winner)
            .WithMany()
            .HasForeignKey(a => a.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Bids)
            .WithOne(b => b.Auction)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}