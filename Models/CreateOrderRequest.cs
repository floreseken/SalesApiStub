using System.ComponentModel.DataAnnotations;

namespace SalesApiStub.Models;

/// <summary>
/// Request payload used to submit a new order.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>Client-side or business identifier for the customer placing the order.</summary>
    [Required]
    [MaxLength(100)]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>Contact email for order confirmations and fulfillment updates.</summary>
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>Shipping address to fulfill this order.</summary>
    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    /// <summary>The list of product lines being ordered.</summary>
    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = [];
}
