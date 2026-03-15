using System.ComponentModel.DataAnnotations;

namespace SalesApiStub.Models;

/// <summary>
/// Represents one product line in an order submission.
/// </summary>
public class OrderItemRequest
{
    /// <summary>The ID of the product being ordered.</summary>
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    /// <summary>The number of units requested for this product.</summary>
    [Range(1, 10_000)]
    public int Quantity { get; set; }
}
