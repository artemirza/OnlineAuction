using Auction.Domain.Enums;

namespace Auction.Domain.Entities;

public sealed class Auction
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AuctionStatus Status { get; set; }
    public Guid CreatorId { get; set; }
    public Guid? WinnerId { get; set; }

    public User Creator { get; set; } = null!;
    public User? Winner { get; set; }
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}