namespace SalesApiStub.Models;

/// <summary>
/// Represents a product available in the sales inventory.
/// </summary>
public class Product
{
    /// <summary>The unique identifier of the product.</summary>
    public int Id { get; set; }

    /// <summary>The display name of the product.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>A brief description of the product.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The product category (e.g. Electronics, Clothing, Books).</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>The retail price of the product.</summary>
    public decimal Price { get; set; }

    /// <summary>The ISO 4217 currency code for the price (e.g. EUR).</summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>The sales unit used for pricing and stock accounting.</summary>
    public Unit Unit { get; set; }

    /// <summary>The number of units currently available in stock.</summary>
    public decimal StockQuantity { get; set; }

    /// <summary>The weight of a single unit of the product in kilograms.</summary>
    public decimal KgsPerUnit { get; set; }
}