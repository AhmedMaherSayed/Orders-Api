using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OrdersApi.Orders;

namespace OrdersApi.IntegrationTests;

public sealed class OrdersEndpointsTests(OrdersApiFactory factory) : IClassFixture<OrdersApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Created_order_can_be_read_back()
    {
        var request = new CreateOrderRequest(
            "customer-42",
            [new OrderLineRequest("SKU-1", 2, 9.99m), new OrderLineRequest("SKU-2", 1, 0.05m)]);

        var created = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.NotNull(created.Headers.Location);

        var fetched = await _client.GetFromJsonAsync<OrderResponse>(created.Headers.Location);

        Assert.NotNull(fetched);
        Assert.Equal("customer-42", fetched.CustomerId);
        Assert.Equal(20.03m, fetched.Total);
        Assert.Equal(2, fetched.Lines.Count);
    }

    [Fact]
    public async Task Invalid_order_is_rejected_with_validation_problem()
    {
        var request = new CreateOrderRequest("customer-42", [new OrderLineRequest("SKU-1", 0, 9.99m)]);

        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement.GetProperty("errors").TryGetProperty("lines[0]", out _));
    }

    [Fact]
    public async Task Unknown_order_returns_404()
    {
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
