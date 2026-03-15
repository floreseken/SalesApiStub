namespace SalesApiStub.Models;

/// <summary>
/// Represents a product available in the sales inventory.
/// </summary>
public class Product
{
    /// <summary>The unique identifier of the product.</summary>
    public int Id { get; set; }

    /// <summary>The Stock Keeping Unit code uniquely identifying this product variant.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>The display name of the product.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>A brief description of the product.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The product category (e.g. Electronics, Clothing, Books).</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>The retail price of the product.</summary>
    public decimal Price { get; set; }

    /// <summary>The ISO 4217 currency code for the price (e.g. USD).</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>The number of units currently available in stock.</summary>
    public int StockQuantity { get; set; }

    /// <summary>Indicates whether this product is currently in stock.</summary>
    public bool InStock => StockQuantity > 0;

    /// <summary>The UTC timestamp of the last inventory update.</summary>
    public DateTime LastUpdated { get; set; }
}
