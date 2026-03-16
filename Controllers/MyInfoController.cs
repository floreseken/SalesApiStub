using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesApiStub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SalesApiStub.Controllers;

/// <summary>
/// Returns identity details for the currently authenticated caller.
/// </summary>
[ApiController]
[Route("api/sales/my-info")]
[Authorize]
[Tags("Sales")]
[Produces("application/json")]
public class MyInfoController : ControllerBase
{
    private static readonly CompanyInfo CompanyProfile = new()
    {
        Name = "Contoso Sales",
        ContactName = "Retail",
        Email = "support@contoso.example"
    };

    private static readonly List<Address> CompanyAddresses =
    [
        new()
        {
            Id = "addr-hq-sea",
            Label = "HQ",
            Line1 = "1 Market St",
            City = "Seattle",
            PostalCode = "98101",
            Country = "US"
        },
        new()
        {
            Id = "addr-warehouse-tac",
            Label = "Warehouse",
            Line1 = "500 Distribution Ave",
            City = "Tacoma",
            PostalCode = "98421",
            Country = "US"
        }
    ];

    /// <summary>
    /// Get identity information derived from the current Bearer token.
    /// </summary>
    /// <returns>The caller identity, granted scopes, expiration, and claims.</returns>
    /// <response code="200">Identity information returned successfully.</response>
    /// <response code="401">Missing or invalid Bearer token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(MyInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<MyInfoResponse> GetMyInfo()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        var clientId = User.FindFirstValue("client_id") ?? string.Empty;
        var scopeClaim = User.FindFirstValue("scope") ?? string.Empty;

        var scopes = scopeClaim
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        DateTime? expiresAtUtc = null;
        var expClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (long.TryParse(expClaim, out var expUnixSeconds))
            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnixSeconds).UtcDateTime;

        var claims = User.Claims
            .GroupBy(c => c.Type, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => string.Join(" | ", group.Select(c => c.Value)),
                StringComparer.Ordinal);

        return Ok(new MyInfoResponse
        {
            Subject = subject,
            ClientId = clientId,
            Scopes = scopes,
            ExpiresAtUtc = expiresAtUtc,
            Claims = claims,
            Company = new CompanyInfo
            {
                Name = CompanyProfile.Name,
                ContactName = CompanyProfile.ContactName,
                Email = CompanyProfile.Email
            },
            Addresses = CompanyAddresses
                .Select(address => new Address
                {
                    Id = address.Id,
                    Label = address.Label,
                    Line1 = address.Line1,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                })
                .ToList()
        });
    }
}
