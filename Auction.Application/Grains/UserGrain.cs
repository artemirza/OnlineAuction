using Auction.Application.Grains.Abs;
using Auction.Domain.Entities;
using Orleans;
using Orleans.Runtime;

namespace Auction.Application.Grains;

public sealed class UserGrain(
    [PersistentState("user", "UserStorage")] IPersistentState<User> grainState
) : Grain, IUserGrain
{
    public Task<decimal> GetBalanceAsync()
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        return Task.FromResult(grainState.State.Balance);
    }

    public Task<decimal> GetBlockedBalanceAsync()
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        return Task.FromResult(grainState.State.BlockedBalance);
    }

    public Task<bool> HasSufficientBalanceAsync(decimal amount)
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        return Task.FromResult(grainState.State.Balance >= amount);
    }

    public async Task BlockBalanceAsync(decimal amount)
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        grainState.State.Balance -= amount;
        grainState.State.BlockedBalance += amount;

        await grainState.WriteStateAsync();
    }

    public async Task UnblockBalanceAsync(decimal amount)
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        if (grainState.State.BlockedBalance < amount)
        {
            throw new InvalidOperationException("Invalid blocked amount");
        }

        grainState.State.BlockedBalance -= amount;
        grainState.State.Balance += amount;

        await grainState.WriteStateAsync();
    }

    public async Task ClearBlockedBalanceAsync()
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        grainState.State.BlockedBalance = 0;
        await grainState.WriteStateAsync();
    }

    public async Task AddBalanceAsync(decimal amount)
    {
        if (!grainState.RecordExists)
            throw new InvalidOperationException("User does not exist");
        
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive");
        }

        grainState.State.Balance += amount;

        await grainState.WriteStateAsync();
    }
}