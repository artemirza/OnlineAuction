using Auction.Application.Repositories;
using Auction.Domain.Enums;
using Auction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public sealed class AuctionRepository(ApplicationDbContext context) : IAuctionRepository
{
    public async Task<Domain.Entities.Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Creator)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auction>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Creator)
            .Where(a => a.Status == AuctionStatus.Active)
            .OrderBy(a => a.EndTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auction>> GetByStatusAsync(AuctionStatus status,
        CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Creator)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Auction>> GetByCreatorIdAsync(Guid creatorId,
        CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Creator)
            .Where(a => a.CreatorId == creatorId)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Auction> CreateAsync(Domain.Entities.Auction auction,
        CancellationToken cancellationToken = default)
    {
        await context.Auctions.AddAsync(auction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return auction;
    }

    public async Task<Domain.Entities.Auction> UpdateAsync(Domain.Entities.Auction auction,
        CancellationToken cancellationToken = default)
    {
        context.Auctions.Update(auction);
        await context.SaveChangesAsync(cancellationToken);
        return auction;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auction = await context.Auctions.FindAsync(new object[] { id }, cancellationToken);
        if (auction != null)
        {
            context.Auctions.Remove(auction);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Auctions.AnyAsync(a => a.Id == id, cancellationToken);
    }
}