using Microsoft.AspNetCore.Mvc;

namespace SalesApiStub.Models;

/// <summary>
/// OAuth 2.0 token request submitted as form data.
/// </summary>
public class TokenRequest
{
    /// <summary>Must be <c>client_credentials</c>.</summary>
    [FromForm(Name = "grant_type")]
    public string GrantType { get; set; } = string.Empty;

    /// <summary>The registered client identifier.</summary>
    [FromForm(Name = "client_id")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>The client secret associated with the client ID.</summary>
    [FromForm(Name = "client_secret")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Optional space-separated list of requested scopes.</summary>
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }
}
