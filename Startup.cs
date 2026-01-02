
namespace BoilerPlait.InventoryService.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureService(IServiceCollection serviceCollection)
        {
            // Add services to the container.

            serviceCollection.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            serviceCollection.AddGrpc();

            serviceCollection.AddGrpcReflection();
        }


        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseHttpsRedirection();

            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGet("", () => "Hello World");
            });
        }
    }
}