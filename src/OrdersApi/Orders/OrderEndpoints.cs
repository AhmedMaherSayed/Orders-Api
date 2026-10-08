using Microsoft.EntityFrameworkCore;

namespace OrdersApi.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/orders");
        orders.MapPost("/", CreateAsync);
        orders.MapGet("/{id:guid}", GetByIdAsync);
        orders.MapGet("/", ListAsync);
        return app;
    }

    private static async Task<IResult> CreateAsync(
        CreateOrderRequest request, OrdersDbContext db, TimeProvider clock, CancellationToken ct)
    {
        var errors = OrderPricing.Validate(request);
        if (errors.Count > 0) return TypedResults.ValidationProblem(errors);

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerId = request.CustomerId,
            CreatedAt = clock.GetUtcNow(),
            Lines = request.Lines
                .Select(l => new OrderLine { Sku = l.Sku, Quantity = l.Quantity, UnitPrice = l.UnitPrice })
                .ToList(),
        };
        order.Total = OrderPricing.Total(order.Lines);

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/orders/{order.Id}", OrderResponse.From(order));
    }

    private static async Task<IResult> GetByIdAsync(Guid id, OrdersDbContext db, CancellationToken ct)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order is null ? TypedResults.NotFound() : TypedResults.Ok(OrderResponse.From(order));
    }

    private static async Task<IResult> ListAsync(OrdersDbContext db, CancellationToken ct)
    {
        var latest = await db.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Take(50)
            .Select(o => new OrderSummary(o.Id, o.CustomerId, o.CreatedAt, o.Total))
            .ToListAsync(ct);

        return TypedResults.Ok(latest);
    }
}
