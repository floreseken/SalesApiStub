using System.Text.Json.Serialization;

namespace SalesApiStub.Models;

/// <summary>
/// OAuth 2.0 error response as defined by RFC 6749.
/// </summary>
public class OAuthError
{
    /// <summary>A short error code string (e.g. <c>invalid_client</c>).</summary>
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>A human-readable description of the error.</summary>
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }
}
