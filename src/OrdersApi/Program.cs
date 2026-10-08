using Microsoft.EntityFrameworkCore;
using OrdersApi.Orders;

var builder = WebApplication.CreateBuilder(args);

// The connection string is read when the DbContext is first resolved, not here.
// That lets tests (and later, containers) supply it through any configuration source.
builder.Services.AddDbContext<OrdersDbContext>((services, options) =>
{
    var connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("Orders")
        ?? throw new InvalidOperationException("Missing configuration 'ConnectionStrings:Orders'.");
    options.UseNpgsql(connectionString);
});
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails();

var app = builder.Build();

// Fail fast: a release with no database configured should die at startup, loudly,
// not on the first customer request.
if (string.IsNullOrWhiteSpace(app.Configuration.GetConnectionString("Orders")))
{
    throw new InvalidOperationException(
        "Missing configuration 'ConnectionStrings:Orders' (environment variable: ConnectionStrings__Orders).");
}

app.UseExceptionHandler();
app.MapOrderEndpoints();

app.Run();

// Lets WebApplicationFactory<Program> in the integration tests find the entry point.
public partial class Program;
