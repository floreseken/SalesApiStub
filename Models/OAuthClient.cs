namespace SalesApiStub.Models;

/// <summary>
/// Represents a registered OAuth 2.0 client loaded from configuration.
/// </summary>
public class OAuthClient
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = [];
}
