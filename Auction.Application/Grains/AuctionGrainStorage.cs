using Auction.Application.Repositories;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;

namespace Auction.Application.Grains;

public sealed class AuctionGrainStorage(
    IAuctionRepository auctionRepository,
    ILogger<AuctionGrainStorage> logger
) : IGrainStorage
{
    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        if (grainState is not IGrainState<Domain.Entities.Auction> auctionGrainState)
        {
            logger.LogError("GrainState is not of type IGrainState<Auction>");
            throw new InvalidCastException("GrainState is not of type IGrainState<Auction>");
        }

        var auctionId = GetAuctionId(grainId);
        var auction = await auctionRepository.GetByIdAsync(auctionId);

        if (auction == null)
        {
            grainState.RecordExists = false;
            auctionGrainState.State = new Domain.Entities.Auction { Id = auctionId };
        }
        else
        {
            grainState.RecordExists = true;
            auctionGrainState.State = auction;
        }
    }

    public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        if (grainState.State is not Domain.Entities.Auction auction)
        {
            logger.LogError("State is not of type Auction");
            throw new InvalidCastException("State is not of type Auction");
        }

        var auctionId = GetAuctionId(grainId);

        if (auction.Id != auctionId)
        {
            logger.LogError("Auction Id {AuctionId} mismatch grain id {GrainId}", auction.Id, auctionId);
            throw new InvalidOperationException($"Auction Id {auction.Id} mismatch grain id {auctionId}");
        }

        if (grainState.RecordExists)
        {
            await auctionRepository.UpdateAsync(auction);
        }
        else
        {
            await auctionRepository.CreateAsync(auction);
            grainState.RecordExists = true;
        }
    }

    public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        // No need in clearing state
        throw new NotImplementedException();
    }

    private static Guid GetAuctionId(GrainId grainId)
    {
        var key = grainId.Key.ToString();

        if (string.IsNullOrEmpty(key) || !Guid.TryParse(key, out var auctionId))
        {
            throw new ArgumentException($"Invalid GrainId: {grainId}");
        }

        return auctionId;
    }
}