FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Auction.Host/Auction.Host.csproj", "Auction.Host/"]
COPY ["Auction.Domain/Auction.Domain.csproj", "Auction.Domain/"]
COPY ["Auction.Infrastructure/Auction.Infrastructure.csproj", "Auction.Infrastructure/"]
COPY ["Auction.Application/Auction.Application.csproj", "Auction.Application/"]
RUN dotnet restore "Auction.Host/Auction.Host.csproj"
COPY . .
WORKDIR "/src/Auction.Host"
RUN dotnet build "./Auction.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auction.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auction.Host.dll"]
