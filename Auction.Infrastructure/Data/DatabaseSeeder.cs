using Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Data;

public sealed class DatabaseSeeder(ApplicationDbContext context)
{
    public async Task SeedAsync()
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "alice@auction.com",
                Balance = 10000m,
                BlockedBalance = 0m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "bob@auction.com",
                Balance = 5000m,
                BlockedBalance = 0m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "charlie@auction.com",
                Balance = 15000m,
                BlockedBalance = 0m
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}