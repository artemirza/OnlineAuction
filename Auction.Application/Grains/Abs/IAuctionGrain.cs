using Auction.Domain.Entities;
using Auction.Domain.Enums;
using Orleans;

namespace Auction.Application.Grains.Abs;

public interface IAuctionGrain : IGrainWithGuidKey
{
    //Task CreateAsync(CreateAuctionRequest request);
    Task<Domain.Entities.Auction?> GetAuctionAsync();
    Task<decimal> GetCurrentPriceAsync();
    Task<AuctionStatus> GetStatusAsync();
    Task<bool> PlaceBidAsync(Guid userId, decimal amount);
    Task<IEnumerable<Bid>> GetBidsAsync();
    Task<Bid?> GetHighestBidAsync();
    Task CloseAuctionAsync();
}