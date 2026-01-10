
using BoilerPlait.InventoryService.Core.Application;
using BoilerPlait.InventoryService.Core.Ports;
using BoilerPlait.InventoryService.Infrastructure.Adapters.MongoDb;
using BoilerPlait.InventoryService.Infrastructure.Adapters.MongoDb.Repository;

namespace BoilerPlait.InventoryService.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            //Grpc
            serviceCollection.AddGrpc();
            serviceCollection.AddGrpcReflection();

            //Mediatr
            serviceCollection.AddMediatR(cfg =>
                 cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

            //MongoDb
            serviceCollection.AddSingleton<MongoDbContext>();

            //services
            serviceCollection.AddScoped<IPartRepository, PartRepository>();
        }
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();

            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<Adapters.Grpc.InventoryService>();
                endpointRouteBuilder.MapGrpcReflectionService();
                endpointRouteBuilder.MapGet("", () => "Hello World");
            });
        }
    }
}