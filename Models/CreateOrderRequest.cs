using System.ComponentModel.DataAnnotations;

namespace SalesApiStub.Models;

/// <summary>
/// Request payload used to submit a new order.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>Shipping address to fulfill this order.</summary>
    [Required]
    [MaxLength(500)]
    public int ShippingAddressId { get; set; }

    /// <summary>The list of product lines being ordered.</summary>
    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = [];
}
