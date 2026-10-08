using OrdersApi.Orders;

namespace OrdersApi.UnitTests;

public sealed class OrderPricingTests
{
    private static CreateOrderRequest Request(string customerId, params OrderLineRequest[] lines) =>
        new(customerId, lines);

    [Fact]
    public void Total_is_sum_of_quantity_times_unit_price()
    {
        OrderLine[] lines =
        [
            new() { Sku = "A", Quantity = 2, UnitPrice = 9.99m },
            new() { Sku = "B", Quantity = 1, UnitPrice = 0.05m },
        ];

        Assert.Equal(20.03m, OrderPricing.Total(lines));
    }

    [Fact]
    public void Total_rounds_midpoint_cents_away_from_zero()
    {
        OrderLine[] lines = [new() { Sku = "A", Quantity = 3, UnitPrice = 0.335m }]; // 1.005

        Assert.Equal(1.01m, OrderPricing.Total(lines));
    }

    [Fact]
    public void Valid_request_has_no_errors()
    {
        var errors = OrderPricing.Validate(Request("customer-1", new OrderLineRequest("A", 1, 1.00m)));

        Assert.Empty(errors);
    }

    [Fact]
    public void Missing_customer_is_rejected()
    {
        var errors = OrderPricing.Validate(Request(" ", new OrderLineRequest("A", 1, 1.00m)));

        Assert.True(errors.ContainsKey("customerId"));
    }

    [Fact]
    public void Order_without_lines_is_rejected()
    {
        var errors = OrderPricing.Validate(Request("customer-1"));

        Assert.True(errors.ContainsKey("lines"));
    }

    [Theory]
    [InlineData("A", 0, 1.00)]     // quantity too low
    [InlineData("A", 1001, 1.00)]  // quantity too high
    [InlineData("A", 1, -0.01)]    // negative price
    [InlineData("", 1, 1.00)]      // missing sku
    public void Invalid_line_is_rejected(string sku, int quantity, double unitPrice)
    {
        var errors = OrderPricing.Validate(
            Request("customer-1", new OrderLineRequest(sku, quantity, (decimal)unitPrice)));

        Assert.True(errors.ContainsKey("lines[0]"));
    }
}
