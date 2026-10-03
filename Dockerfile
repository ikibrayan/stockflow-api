FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/StockFlow.Api/StockFlow.Api.csproj", "src/StockFlow.Api/"]
COPY ["src/StockFlow.Application/StockFlow.Application.csproj", "src/StockFlow.Application/"]
COPY ["src/StockFlow.Domain/StockFlow.Domain.csproj", "src/StockFlow.Domain/"]
COPY ["src/StockFlow.Infrastructure/StockFlow.Infrastructure.csproj", "src/StockFlow.Infrastructure/"]

RUN dotnet restore "src/StockFlow.Api/StockFlow.Api.csproj"

COPY . .

WORKDIR "/src/src/StockFlow.Api"

RUN dotnet publish "StockFlow.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StockFlow.Api.dll"]