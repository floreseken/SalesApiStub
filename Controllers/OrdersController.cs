using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesApiStub.Models;
using SalesApiStub.Services;

namespace SalesApiStub.Controllers;

/// <summary>
/// Provides order submission and retrieval endpoints.
/// </summary>
[ApiController]
[Route("api/sales/orders")]
[Authorize]
[Tags("Sales")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService) => _orderService = orderService;

    /// <summary>
    /// Submit a new order.
    /// </summary>
    /// <remarks>
    /// Validates that all requested products exist and have available stock.
    /// If accepted, stock is reserved immediately and an order confirmation is returned.
    /// </remarks>
    /// <param name="request">The order details and requested product lines.</param>
    /// <returns>The created order confirmation payload.</returns>
    /// <response code="201">Order created successfully.</response>
    /// <response code="400">Invalid order payload, missing products, or insufficient stock.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<OrderResponse> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!_orderService.TryCreateOrder(request, out var order, out var errorMessage))
            return BadRequest(new { message = errorMessage ?? "Unable to create order." });

        return CreatedAtAction(nameof(GetOrder), new { id = order!.OrderId }, order);
    }

    /// <summary>
    /// Retrieve a previously submitted order by ID.
    /// </summary>
    /// <param name="id">The unique order identifier.</param>
    /// <returns>The order details if found.</returns>
    /// <response code="200">Order found and returned.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<OrderResponse> GetOrder(Guid id)
    {
        var order = _orderService.GetById(id);

        if (order is null)
            return NotFound(new { message = $"Order '{id}' was not found." });

        return Ok(order);
    }
}
