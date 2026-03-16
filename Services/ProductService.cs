using SalesApiStub.Models;

namespace SalesApiStub.Services;

/// <summary>
/// In-memory stub data service providing product inventory and pricing information.
/// </summary>
public class ProductService
{
    private static readonly object SyncRoot = new();

    private static readonly List<Product> Products =
    [
        new()
        {
            Id = 1, Name = "Wireless Headphones Pro",
            Description = "High-fidelity wireless headphones with active noise cancellation and 30-hour battery life.",
            Category = "Electronics", Price = 299.99m, Currency = "USD",
            StockQuantity = 45
        },
        new()
        {
            Id = 2, Name = "Smart Watch Series X",
            Description = "Advanced smartwatch with health tracking, GPS, and 7-day battery life.",
            Category = "Electronics", Price = 449.99m, Currency = "USD",
            StockQuantity = 30
        },
        new()
        {
            Id = 3, Name = "Portable Bluetooth Speaker",
            Description = "Waterproof outdoor speaker with 360° sound and 20-hour playback.",
            Category = "Electronics", Price = 129.99m, Currency = "USD",
            StockQuantity = 120
        },
        new()
        {
            Id = 4, Name = "4K Webcam Ultra",
            Description = "4K resolution webcam with built-in ring light and auto-focus for professional video calls.",
            Category = "Electronics", Price = 159.99m, Currency = "USD",
            StockQuantity = 0
        },
        new()
        {
            Id = 5, Name = "Men's Performance Jacket",
            Description = "Lightweight windproof jacket with moisture-wicking fabric. Available in multiple sizes.",
            Category = "Clothing", Price = 89.99m, Currency = "USD",
            StockQuantity = 75
        },
        new()
        {
            Id = 6, Name = "Athletic Sneakers",
            Description = "Breathable running shoes with memory foam insole and durable rubber outsole.",
            Category = "Clothing", Price = 119.99m, Currency = "USD",
            StockQuantity = 60
        },
        new()
        {
            Id = 7, Name = "Smart Coffee Maker",
            Description = "Programmable coffee maker with smartphone connectivity and built-in grinder.",
            Category = "Home & Kitchen", Price = 199.99m, Currency = "USD",
            StockQuantity = 25
        },
        new()
        {
            Id = 8, Name = "Air Purifier HEPA Plus",
            Description = "True HEPA filter air purifier covering up to 600 sq ft with auto mode and air quality display.",
            Category = "Home & Kitchen", Price = 279.99m, Currency = "USD",
            StockQuantity = 18
        },
        new()
        {
            Id = 9, Name = "Clean Code: A Handbook",
            Description = "Essential guide to writing clean, maintainable, and readable code by Robert C. Martin.",
            Category = "Books", Price = 34.99m, Currency = "USD",
            StockQuantity = 200
        },
        new()
        {
            Id = 10, Name = "Domain-Driven Design",
            Description = "Tackling complexity in the heart of software by Eric Evans. A must-read for architects.",
            Category = "Books", Price = 49.99m, Currency = "USD",
            StockQuantity = 85
        },
        new()
        {
            Id = 11, Name = "Yoga Mat Premium",
            Description = "Non-slip 6mm thick yoga mat with alignment lines, carrying strap, and eco-friendly material.",
            Category = "Sports", Price = 59.99m, Currency = "USD",
            StockQuantity = 150
        },
        new()
        {
            Id = 12, Name = "Adjustable Dumbbell Set",
            Description = "Space-saving adjustable dumbbells ranging from 5 to 52.5 lbs with quick-release dial.",
            Category = "Sports", Price = 349.99m, Currency = "USD",
            StockQuantity = 15
        },
    ];

    /// <summary>Returns all products regardless of stock status.</summary>
    public IReadOnlyList<Product> GetAll() => Products.AsReadOnly();

    /// <summary>Returns only products that are currently in stock (StockQuantity > 0).</summary>
    public IReadOnlyList<Product> GetInStock() =>
        Products.Where(p => p.StockQuantity > 0).ToList().AsReadOnly();

    /// <summary>Returns a single product by its ID, or <c>null</c> if not found.</summary>
    public Product? GetById(int id) => Products.FirstOrDefault(p => p.Id == id);

    /// <summary>Returns all distinct category names, ordered alphabetically.</summary>
    public IReadOnlyList<string> GetCategories() =>
        Products.Select(p => p.Category).Distinct().Order().ToList().AsReadOnly();

    /// <summary>Returns in-stock products filtered by category (case-insensitive).</summary>
    public IReadOnlyList<Product> GetByCategory(string category) =>
        Products
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.StockQuantity > 0)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Attempts to reserve stock for all requested lines atomically.
    /// </summary>
    public bool TryReserveStock(
        IReadOnlyList<OrderItemRequest> orderItems,
        out IReadOnlyList<Product> reservedProducts,
        out string? errorMessage)
    {
        reservedProducts = [];
        errorMessage = null;

        lock (SyncRoot)
        {
            var normalizedItems = orderItems
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var productsById = new Dictionary<int, Product>();

            foreach (var item in normalizedItems)
            {
                var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product is null)
                {
                    errorMessage = $"Product with ID {item.ProductId} was not found.";
                    return false;
                }

                if (product.StockQuantity < item.Quantity)
                {
                    errorMessage =
                        $"Insufficient stock for product '{product.Name}' (ID {product.Id}). Requested {item.Quantity}, available {product.StockQuantity}.";
                    return false;
                }

                productsById[item.ProductId] = product;
            }

            foreach (var item in normalizedItems)
            {
                var product = productsById[item.ProductId];
                product.StockQuantity -= item.Quantity;
            }

            reservedProducts = productsById.Values.ToList().AsReadOnly();
            return true;
        }
    }
}
