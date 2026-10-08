namespace OrdersApi.Orders;

public sealed class Order
{
    public Guid Id { get; set; }
    public required string CustomerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal Total { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}

public sealed class OrderLine
{
    public int Id { get; set; }
    public required string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
