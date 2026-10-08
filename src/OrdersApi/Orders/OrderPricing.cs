namespace OrdersApi.Orders;

/// <summary>Pure business rules: no I/O, so they are unit-tested without a database.</summary>
public static class OrderPricing
{
    public const int MaxLines = 50;
    public const int MaxQuantity = 1000;
    public const int MaxIdLength = 64;

    public static Dictionary<string, string[]> Validate(CreateOrderRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            errors["customerId"] = ["Customer id is required."];
        }
        else if (request.CustomerId.Length > MaxIdLength)
        {
            errors["customerId"] = [$"Customer id must be {MaxIdLength} characters or fewer."];
        }

        if (request.Lines is null || request.Lines.Count == 0)
        {
            errors["lines"] = ["An order needs at least one line."];
            return errors;
        }

        if (request.Lines.Count > MaxLines)
        {
            errors["lines"] = [$"An order can have at most {MaxLines} lines."];
        }

        for (var i = 0; i < request.Lines.Count; i++)
        {
            var line = request.Lines[i];
            if (line is null)
            {
                errors[$"lines[{i}]"] = ["Line is required."];
                continue;
            }

            var lineErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(line.Sku)) lineErrors.Add("Sku is required.");
            else if (line.Sku.Length > MaxIdLength) lineErrors.Add($"Sku must be {MaxIdLength} characters or fewer.");
            if (line.Quantity is < 1 or > MaxQuantity) lineErrors.Add($"Quantity must be between 1 and {MaxQuantity}.");
            if (line.UnitPrice < 0) lineErrors.Add("Unit price can't be negative.");

            if (lineErrors.Count > 0) errors[$"lines[{i}]"] = [.. lineErrors];
        }

        return errors;
    }

    /// <summary>Sum of quantity x unit price, rounded to cents (midpoint away from zero).</summary>
    public static decimal Total(IEnumerable<OrderLine> lines) =>
        Math.Round(lines.Sum(l => l.Quantity * l.UnitPrice), 2, MidpointRounding.AwayFromZero);
}
