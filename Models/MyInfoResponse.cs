namespace SalesApiStub.Models;

/// <summary>
/// Represents identity details extracted from the current Bearer token.
/// </summary>
public class MyInfoResponse
{
    /// <summary>The subject claim from the token.</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>The OAuth client identifier.</summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>All granted OAuth scopes.</summary>
    public List<string> Scopes { get; set; } = [];

    /// <summary>The UTC timestamp at which the token expires, if present.</summary>
    public DateTime? ExpiresAtUtc { get; set; }

    /// <summary>The raw claim collection for troubleshooting/demo purposes.</summary>
    public Dictionary<string, string> Claims { get; set; } = [];

    /// <summary>Basic information about the company associated with this API.</summary>
    public CompanyInfo Company { get; set; } = new();

    /// <summary>A list of known company addresses.</summary>
    public List<Address> KnownAddresses { get; set; } = [];
}
