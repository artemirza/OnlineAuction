using Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Domain.Entities.Auction> Auctions => Set<Domain.Entities.Auction>();
    public DbSet<Bid> Bids => Set<Bid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}