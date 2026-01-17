namespace Auction.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal BlockedBalance { get; set; }
    
    public ICollection<Auction> CreatedAuctions { get; set; } = new List<Auction>();
    
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}