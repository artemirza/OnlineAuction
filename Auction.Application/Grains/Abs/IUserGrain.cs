using Orleans;

namespace Auction.Application.Grains.Abs;

public interface IUserGrain : IGrainWithGuidKey
{
    Task<decimal> GetBalanceAsync();
    Task<decimal> GetBlockedBalanceAsync();
    Task<bool> HasSufficientBalanceAsync(decimal amount);
    Task BlockBalanceAsync(decimal amount);
    Task UnblockBalanceAsync(decimal amount);
    Task ClearBlockedBalanceAsync();
    Task AddBalanceAsync(decimal amount);
}