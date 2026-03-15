using System.Text.Json.Serialization;

namespace SalesApiStub.Models;

/// <summary>
/// OAuth 2.0 access token response.
/// </summary>
public class TokenResponse
{
    /// <summary>The issued JWT Bearer token.</summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Always <c>Bearer</c>.</summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    /// <summary>Seconds until the token expires.</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>The space-separated list of granted scopes.</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}
