using Auction.Application.Repositories;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;

namespace Auction.Application.Grains;

public sealed class UserGrainStorage(
    IUserRepository userRepository,
    ILogger<UserGrainStorage> logger
) : IGrainStorage
{
    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        if (grainState is not IGrainState<User> userGrainState)
        {
            logger.LogError("GrainState is not of type IGrainState<User>");
            throw new InvalidCastException("GrainState is not of type IGrainState<User>");
        }

        var userId = GetUserId(grainId);
        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            grainState.RecordExists = false;
            userGrainState.State = new User { Id = userId };
        }
        else
        {
            grainState.RecordExists = true;
            userGrainState.State = user;
        }
    }

    public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        if (grainState.State is not User user)
        {
            logger.LogError("State is not of type User");
            throw new InvalidCastException("State is not of type User");
        }

        var userId = GetUserId(grainId);

        if (user.Id != userId)
        {
            logger.LogError("User Id {UserId} mismatch grain id {GrainId}", user.Id, userId);
            throw new InvalidOperationException($"User Id {user.Id} mismatch grain id {userId}");
        }

        if (grainState.RecordExists)
        {
            await userRepository.UpdateAsync(user);
        }
        else
        {
            await userRepository.CreateAsync(user);
            grainState.RecordExists = true;
        }
    }

    public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        // No need in clearing state
        throw new NotImplementedException();
    }

    private static Guid GetUserId(GrainId grainId)
    {
        var key = grainId.Key.ToString();

        if (string.IsNullOrEmpty(key) || !Guid.TryParse(key, out var userId))
        {
            throw new ArgumentException($"Invalid GrainId: {grainId}");
        }

        return userId;
    }
}