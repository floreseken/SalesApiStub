using Microsoft.AspNetCore.Mvc;
using SalesApiStub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SalesApiStub.Controllers;

/// <summary>
/// Issues OAuth 2.0 access tokens via the Client Credentials grant (RFC 6749 §4.4).
/// </summary>
[ApiController]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config) => _config = config;

    /// <summary>
    /// Request an OAuth 2.0 Bearer access token using the Client Credentials grant.
    /// </summary>
    /// <remarks>
    /// Submit <c>application/x-www-form-urlencoded</c> body with the following fields:
    ///
    /// | Field           | Value               |
    /// |-----------------|---------------------|
    /// | grant_type      | client_credentials  |
    /// | client_id       | demo-client         |
    /// | client_secret   | demo-secret         |
    /// | scope           | sales:read          |
    ///
    /// The returned `access_token` must be passed as a `Bearer` token in the
    /// `Authorization` header of subsequent API requests.
    /// </remarks>
    /// <param name="request">The token request form fields.</param>
    /// <returns>A JWT Bearer token on success.</returns>
    /// <response code="200">Token issued successfully.</response>
    /// <response code="400">Invalid grant type or scope.</response>
    /// <response code="401">Invalid client credentials.</response>
    [HttpPost("/oauth/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OAuthError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OAuthError), StatusCodes.Status401Unauthorized)]
    public IActionResult Token([FromForm] TokenRequest request)
    {
        if (!string.Equals(request.GrantType, "client_credentials", StringComparison.Ordinal))
        {
            return BadRequest(new OAuthError
            {
                Error = "unsupported_grant_type",
                ErrorDescription = "Only 'client_credentials' grant type is supported."
            });
        }

        var clients = _config.GetSection("OAuth:Clients").Get<List<OAuthClient>>() ?? [];
        var matchedClient = clients.FirstOrDefault(c =>
            c.ClientId == request.ClientId &&
            ConstantTimeEquals(c.ClientSecret, request.ClientSecret));

        if (matchedClient is null)
        {
            return Unauthorized(new OAuthError
            {
                Error = "invalid_client",
                ErrorDescription = "The client credentials are invalid."
            });
        }

        var requestedScopes = string.IsNullOrWhiteSpace(request.Scope)
            ? matchedClient.Scopes
            : request.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Intersect(matchedClient.Scopes)
                           .ToList();

        if (requestedScopes.Count == 0)
        {
            return BadRequest(new OAuthError
            {
                Error = "invalid_scope",
                ErrorDescription = "None of the requested scopes are permitted for this client."
            });
        }

        var token = IssueToken(matchedClient.ClientId, requestedScopes);
        return Ok(token);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private TokenResponse IssueToken(string clientId, IEnumerable<string> scopes)
    {
        var secretKey   = _config["OAuth:SecretKey"]!;
        var issuer      = _config["OAuth:Issuer"]!;
        var audience    = _config["OAuth:Audience"]!;
        var expirySeconds = _config.GetValue<int>("OAuth:TokenExpirationSeconds", 3600);

        var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var grantedScopes = string.Join(" ", scopes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, clientId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("client_id", clientId),
            new("scope", grantedScopes),
        };

        var jwt = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddSeconds(expirySeconds),
            signingCredentials: credentials);

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            TokenType   = "Bearer",
            ExpiresIn   = expirySeconds,
            Scope       = grantedScopes
        };
    }

    /// <summary>
    /// Compares two strings in constant time to mitigate timing-based side-channel attacks.
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
