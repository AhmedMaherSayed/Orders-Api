using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersApi.Orders;
using Testcontainers.PostgreSql;

namespace OrdersApi.IntegrationTests;

/// <summary>
/// Starts a throwaway Postgres container, points the real app at it, and applies the
/// real EF migrations. Needs a running Docker daemon.
/// </summary>
public sealed class OrdersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
 private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17.6-alpine")
    .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // "Testing" means appsettings.Development.json is NOT loaded,
        // so the only connection string is the container's.
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Orders"] = _postgres.GetConnectionString(),
            }));
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await db.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
