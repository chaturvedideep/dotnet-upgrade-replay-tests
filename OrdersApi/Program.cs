using System.Globalization;
using System.Text;

var app = WebApplication.CreateBuilder(args).Build();

// A small stand-in for a partner-facing API. The partner parses these responses by hand,
// so the exact shape matters more than whether it is "good" JSON.
var orders = new Dictionary<int, Order>
{
    [1001] = new(1001, "ACME Ltd", new DateTime(2026, 3, 14, 9, 30, 0, DateTimeKind.Utc), 1249.50m, "SHIPPED"),
    [1002] = new(1002, "Northwind Traders", new DateTime(2026, 3, 15, 16, 5, 0, DateTimeKind.Utc), 89.00m, "PENDING"),
};

app.MapGet("/orders/{id:int}", (int id) =>
    orders.TryGetValue(id, out var o)
        ? Results.Ok(o)
        : Results.NotFound(new { error = "order_not_found", id }));

// Nightly export a partner imports. Column order and date format are part of the contract.
app.MapGet("/orders/export.csv", () =>
{
    var sb = new StringBuilder("OrderId,Customer,PlacedOn,Total,Status\n");
    foreach (var o in orders.Values.OrderBy(o => o.Id))
        sb.Append(string.Create(CultureInfo.InvariantCulture,
            $"{o.Id},{o.Customer},{o.PlacedOn:yyyy-MM-dd},{o.Total:0.00},{o.Status}\n"));
    return Results.Text(sb.ToString(), "text/csv");
});

app.Run();

public record Order(int Id, string Customer, DateTime PlacedOn, decimal Total, string Status);

public partial class Program { }
