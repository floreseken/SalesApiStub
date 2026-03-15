using SalesApiStub.Models;

namespace SalesApiStub.Services;

/// <summary>
/// Handles in-memory order creation, stock reservation, and order retrieval.
/// </summary>
public class OrderService
{
    private readonly ProductService _productService;
    private readonly List<OrderResponse> _orders = [];
    private readonly object _syncRoot = new();

    public OrderService(ProductService productService) => _productService = productService;

    /// <summary>
    /// Creates an order when all requested products exist and have enough stock.
    /// </summary>
    public bool TryCreateOrder(CreateOrderRequest request, out OrderResponse? order, out string? errorMessage)
    {
        order = null;
        errorMessage = null;

        if (request.Items.Count == 0)
        {
            errorMessage = "Order must contain at least one item.";
            return false;
        }

        var normalizedItems = request.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new OrderItemRequest { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        if (!_productService.TryReserveStock(normalizedItems, out var reservedProducts, out errorMessage))
            return false;

        var productMap = reservedProducts.ToDictionary(p => p.Id);
        var lines = normalizedItems.Select(item =>
        {
            var product = productMap[item.ProductId];
            var lineTotal = decimal.Round(product.Price * item.Quantity, 2, MidpointRounding.AwayFromZero);

            return new OrderLineResponse
            {
                ProductId = product.Id,
                Sku = product.Sku,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            };
        }).ToList();

        var currencies = reservedProducts.Select(p => p.Currency).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (currencies.Count != 1)
        {
            errorMessage = "Order contains products with mixed currencies, which is not supported.";
            return false;
        }

        var subtotal = decimal.Round(lines.Sum(l => l.LineTotal), 2, MidpointRounding.AwayFromZero);
        var tax = decimal.Round(subtotal * 0.08m, 2, MidpointRounding.AwayFromZero);
        var total = subtotal + tax;

        var createdOrder = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CustomerEmail = request.CustomerEmail,
            ShippingAddress = request.ShippingAddress,
            Currency = currencies[0].ToUpperInvariant(),
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            CreatedAtUtc = DateTime.UtcNow,
            Items = lines
        };

        lock (_syncRoot)
        {
            _orders.Add(createdOrder);
        }

        order = createdOrder;
        return true;
    }

    /// <summary>
    /// Returns a previously created order by ID.
    /// </summary>
    public OrderResponse? GetById(Guid orderId)
    {
        lock (_syncRoot)
        {
            return _orders.FirstOrDefault(o => o.OrderId == orderId);
        }
    }
}
