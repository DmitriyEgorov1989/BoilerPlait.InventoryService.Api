FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src
COPY ["BoilerPlait.InventoryService.Api/BoilerPlait.InventoryService.Api.csproj", "BoilerPlait.InventoryService.Api/"]
COPY ["BoilerPlait.InventoryService.Core/BoilerPlait.InventoryService.Core.csproj", "BoilerPlait.InventoryService.Core/"]
COPY ["BoilerPlait.InventoryService.Infrastructure/BoilerPlait.InventoryService.Infrastructure.csproj", "BoilerPlait.InventoryService.Infrastructure/"]
COPY ["Primitives/Primitives.csproj", "Primitives/"]
RUN dotnet restore "BoilerPlait.InventoryService.Api/BoilerPlait.InventoryService.Api.csproj"
COPY . .
RUN dotnet publish "BoilerPlait.InventoryService.Api/BoilerPlait.InventoryService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN groupadd -g 10001 appgroup \
 && useradd -u 10001 -g appgroup -s /usr/sbin/nologin -m appuser

COPY --from=builder /app/publish/ ./
RUN chown -R appuser:appgroup /app

USER appuser
ENV ASPNETCORE_URLS=http://+:5005
EXPOSE 5005
ENTRYPOINT ["dotnet", "BoilerPlait.InventoryService.Api.dll"]