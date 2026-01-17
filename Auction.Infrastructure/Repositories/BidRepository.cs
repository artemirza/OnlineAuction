using Auction.Application.Repositories;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public sealed class BidRepository(ApplicationDbContext context) : IBidRepository
{
    public async Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Include(b => b.User)
            .Include(b => b.Auction)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Bid>> GetByAuctionIdAsync(Guid auctionId,
        CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Include(b => b.User)
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bid>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Include(b => b.Auction)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bid?> GetHighestBidForAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Include(b => b.User)
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .ThenByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bid>> GetUserBidsForAuctionAsync(Guid userId, Guid auctionId,
        CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Where(b => b.UserId == userId && b.AuctionId == auctionId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bid> CreateAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        bid.CreatedAt = DateTime.UtcNow;
        await context.Bids.AddAsync(bid, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return bid;
    }

    public async Task<Bid> UpdateAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        context.Bids.Update(bid);
        await context.SaveChangesAsync(cancellationToken);
        return bid;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bid = await context.Bids.FindAsync(new object[] { id }, cancellationToken);
        if (bid != null)
        {
            context.Bids.Remove(bid);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}