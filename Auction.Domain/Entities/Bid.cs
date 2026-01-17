using Auction.Domain.Enums;

namespace Auction.Domain.Entities;

public sealed class Bid
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public BidStatus Status { get; set; }

    public Auction Auction { get; set; } = null!;
    public User User { get; set; } = null!;
}