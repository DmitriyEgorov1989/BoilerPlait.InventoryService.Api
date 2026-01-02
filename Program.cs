using BoilerPlait.InventoryService.Api;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

public sealed class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
            .ConfigureKestrel(option =>
            {
                option.ListenPortByOptions(ProgramExtensions.INVENTORYSERVICE_GRPC_PORT, HttpProtocols.Http2);
                option.ListenPortByOptions(ProgramExtensions.INVENTORYSERVICE_HTTP_PORT, HttpProtocols.Http1);
            }))
            .Build()
            .Run();
    }

}
public static class ProgramExtensions
{
    public const string INVENTORYSERVICE_GRPC_PORT = "INVENTORYSERVICE_GRPC_PORT";
    public const string INVENTORYSERVICE_HTTP_PORT = "INVENTORYSERVICE_HTTP_PORT";
    public static void ListenPortByOptions(
        this KestrelServerOptions option,
        string envOption,
        HttpProtocols httpProtocols)
    {
        var isHttpParsed = int.TryParse(Environment.GetEnvironmentVariable(envOption), out var httpPort);

        if (isHttpParsed)
        {
            option.Listen(IPAddress.Any, httpPort, option => option.Protocols = httpProtocols);
        }
    }
}