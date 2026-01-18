using Auction.Application.Repositories;
using Auction.Infrastructure.Data;
using Auction.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Storage;

namespace Auction.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        
        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddSingleton<IGrainStorage>(sp => 
            sp.GetRequiredKeyedService<IGrainStorage>("UserStorage"));
        services.AddSingleton<IGrainStorage>(sp => 
            sp.GetRequiredKeyedService<IGrainStorage>("AuctionStorage"));

        return services;
    }
}