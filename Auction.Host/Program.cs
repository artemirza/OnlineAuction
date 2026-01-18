using Auction.Application.Grains;
using Auction.Infrastructure;
using Auction.Infrastructure.Data;
using Orleans.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddStorage();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder.UseLocalhostClustering();
    
    siloBuilder.Services.AddSingleton<UserGrainStorage>();
    siloBuilder.Services.AddSingleton<AuctionGrainStorage>();

    siloBuilder.Services.AddKeyedSingleton<IGrainStorage>("UserStorage", 
        (sp, key) => sp.GetRequiredService<UserGrainStorage>());
    
    siloBuilder.Services.AddKeyedSingleton<IGrainStorage>("AuctionStorage", 
        (sp, key) => sp.GetRequiredService<AuctionGrainStorage>());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();