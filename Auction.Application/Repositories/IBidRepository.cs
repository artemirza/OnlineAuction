using Auction.Domain.Entities;

namespace Auction.Application.Repositories;

public interface IBidRepository
{
    Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bid>> GetByAuctionIdAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bid>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Bid?> GetHighestBidForAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bid>> GetUserBidsForAuctionAsync(Guid userId, Guid auctionId, CancellationToken cancellationToken = default);
    Task<Bid> CreateAsync(Bid bid, CancellationToken cancellationToken = default);
    Task<Bid> UpdateAsync(Bid bid, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}