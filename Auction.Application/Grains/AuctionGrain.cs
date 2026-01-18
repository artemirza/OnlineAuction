using Auction.Application.Grains.Abs;
using Auction.Domain.Entities;
using Auction.Domain.Enums;
using Orleans;
using Orleans.Runtime;
using AuctionEntity = Auction.Domain.Entities.Auction;

namespace Auction.Application.Grains;

public sealed class AuctionGrain(
    [PersistentState("auction", "AuctionStorage")] IPersistentState<AuctionEntity> grainState
) : Grain, IAuctionGrain
{
    public Task<AuctionEntity?> GetAuctionAsync()
    {
        if (!grainState.RecordExists)
            return Task.FromResult<AuctionEntity?>(null);

        return Task.FromResult<AuctionEntity?>(grainState.State);
    }

    public Task<decimal> GetCurrentPriceAsync()
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("Auction does not exist");
        
        return Task.FromResult(grainState.State.CurrentPrice);
    }

    public Task<AuctionStatus> GetStatusAsync()
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("Auction does not exist");
        
        return Task.FromResult(grainState.State.Status);
    }
    
    // public async Task CreateAsync(CreateAuctionRequest request)
    // {
    //     if (grainState.RecordExists)
    //         throw new InvalidOperationException("Auction already exists");
    //
    //     grainState.State = new AuctionEntity
    //     {
    //         Id = this.GetPrimaryKey(),
    //         Title = request.Title,
    //         Description = request.Description,
    //         StartingPrice = request.StartingPrice,
    //         CurrentPrice = request.StartingPrice,
    //         CreatorId = request.CreatorId,
    //         StartTime = request.StartTime,
    //         EndTime = request.EndTime,
    //         Status = AuctionStatus.Active
    //     };
    //
    //     await grainState.WriteStateAsync();
    // }

    public async Task<bool> PlaceBidAsync(Guid userId, decimal amount)
    {
        if (!grainState.RecordExists)
            return false;

        if (grainState.State.Status != AuctionStatus.Active)
            return false;

        if (amount <= grainState.State.CurrentPrice)
            return false;

        if (grainState.State.CreatorId == userId)
            return false;

        var userGrain = GrainFactory.GetGrain<IUserGrain>(userId);

        if (!await userGrain.HasSufficientBalanceAsync(amount))
            return false;

        var previousHighestBid = await GetHighestBidAsync();

        await userGrain.BlockBalanceAsync(amount);

        if (previousHighestBid != null)
        {
            var previousBidderGrain = GrainFactory.GetGrain<IUserGrain>(previousHighestBid.UserId);
            await previousBidderGrain.UnblockBalanceAsync(previousHighestBid.Amount);

            previousHighestBid.Status = BidStatus.Outbid;
        }

        var bid = new Bid
        {
            Id = Guid.NewGuid(),
            AuctionId = grainState.State.Id,
            UserId = userId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            Status = BidStatus.Active
        };

        grainState.State.Bids.Add(bid);
        grainState.State.CurrentPrice = amount;

        await grainState.WriteStateAsync();

        return true;
    }

    public Task<IEnumerable<Bid>> GetBidsAsync()
    {
        if (!grainState.RecordExists)
            return Task.FromResult(Enumerable.Empty<Bid>());

        return Task.FromResult(grainState.State.Bids.AsEnumerable());
    }

    public Task<Bid?> GetHighestBidAsync()
    {
        if (!grainState.RecordExists)
            return Task.FromResult<Bid?>(null);

        var highestBid = grainState.State.Bids
            .Where(b => b.Status == BidStatus.Active)
            .OrderByDescending(b => b.Amount)
            .ThenByDescending(b => b.CreatedAt)
            .FirstOrDefault();

        return Task.FromResult(highestBid);
    }

    public async Task CloseAuctionAsync()
    {
        if (!grainState.RecordExists)
            return;

        if (grainState.State.Status != AuctionStatus.Active)
            return;

        grainState.State.Status = AuctionStatus.Closed;

        var highestBid = await GetHighestBidAsync();

        if (highestBid != null)
        {
            grainState.State.WinnerId = highestBid.UserId;

            var winnerGrain = GrainFactory.GetGrain<IUserGrain>(highestBid.UserId);
            await winnerGrain.ClearBlockedBalanceAsync();

            highestBid.Status = BidStatus.Won;
        }

        await grainState.WriteStateAsync();
    }
}