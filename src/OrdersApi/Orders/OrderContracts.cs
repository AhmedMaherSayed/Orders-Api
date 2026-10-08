namespace OrdersApi.Orders;

public sealed record CreateOrderRequest(string CustomerId, IReadOnlyList<OrderLineRequest> Lines);

public sealed record OrderLineRequest(string Sku, int Quantity, decimal UnitPrice);

public sealed record OrderLineResponse(string Sku, int Quantity, decimal UnitPrice);

public sealed record OrderResponse(
    Guid Id,
    string CustomerId,
    DateTimeOffset CreatedAt,
    decimal Total,
    IReadOnlyList<OrderLineResponse> Lines)
{
    public static OrderResponse From(Order order) => new(
        order.Id,
        order.CustomerId,
        order.CreatedAt,
        order.Total,
        order.Lines.Select(l => new OrderLineResponse(l.Sku, l.Quantity, l.UnitPrice)).ToList());
}

public sealed record OrderSummary(Guid Id, string CustomerId, DateTimeOffset CreatedAt, decimal Total);
