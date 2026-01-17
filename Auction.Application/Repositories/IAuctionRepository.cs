using Auction.Domain.Enums;

namespace Auction.Application.Repositories;

public interface IAuctionRepository
{
    Task<Domain.Entities.Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Auction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Auction>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Auction>> GetByStatusAsync(AuctionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Auction>> GetByCreatorIdAsync(Guid creatorId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Auction> CreateAsync(Domain.Entities.Auction auction, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Auction> UpdateAsync(Domain.Entities.Auction auction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
