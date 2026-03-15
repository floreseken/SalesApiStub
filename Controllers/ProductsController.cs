using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesApiStub.Models;
using SalesApiStub.Services;

namespace SalesApiStub.Controllers;

/// <summary>
/// Provides access to sales inventory: products currently in stock with their prices.
/// All endpoints require a valid OAuth 2.0 Bearer token with the <c>sales:read</c> scope.
/// </summary>
[ApiController]
[Route("api/sales/products")]
[Authorize]
[Tags("Sales")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService) =>
        _productService = productService;

    /// <summary>
    /// Retrieve all products currently in stock.
    /// </summary>
    /// <remarks>
    /// Returns products with a <c>StockQuantity</c> greater than zero.
    /// Optionally filter by <c>category</c> using the query parameter.
    ///
    /// **Example categories:** Electronics, Clothing, Books, Sports, Home &amp; Kitchen
    /// </remarks>
    /// <param name="category">Optional. Filter results to a specific product category (case-insensitive).</param>
    /// <returns>A list of in-stock products with their current prices.</returns>
    /// <response code="200">Returns the list of in-stock products.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IReadOnlyList<Product>> GetInStockProducts([FromQuery] string? category = null)
    {
        var products = string.IsNullOrWhiteSpace(category)
            ? _productService.GetInStock()
            : _productService.GetByCategory(category);

        return Ok(products);
    }

    /// <summary>
    /// Retrieve a specific product by its unique ID.
    /// </summary>
    /// <param name="id">The unique numeric identifier of the product.</param>
    /// <returns>The full product details including price and current stock quantity.</returns>
    /// <response code="200">Product found and returned.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    /// <response code="404">No product exists with the given ID.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _productService.GetById(id);

        if (product is null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        return Ok(product);
    }

    /// <summary>
    /// Retrieve all available product category names.
    /// </summary>
    /// <remarks>
    /// Use the returned values as the <c>category</c> query parameter on
    /// <c>GET /api/sales/products</c> to filter the product list.
    /// </remarks>
    /// <returns>An alphabetically sorted list of distinct category names.</returns>
    /// <response code="200">Returns the list of available categories.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IReadOnlyList<string>> GetCategories() =>
        Ok(_productService.GetCategories());
}
