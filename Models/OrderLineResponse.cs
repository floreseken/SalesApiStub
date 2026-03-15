namespace SalesApiStub.Models;

/// <summary>
/// Represents a priced line item returned in a created order response.
/// </summary>
public class OrderLineResponse
{
    /// <summary>The product ID for this line.</summary>
    public int ProductId { get; set; }

    /// <summary>The product SKU for this line.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>The display name of the product.</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>The quantity included for this line.</summary>
    public int Quantity { get; set; }

    /// <summary>The unit price captured at order creation time.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>The line total equal to Quantity multiplied by UnitPrice.</summary>
    public decimal LineTotal { get; set; }
}
