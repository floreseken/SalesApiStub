namespace SalesApiStub.Models;

/// <summary>
/// Represents basic company profile details.
/// </summary>
public class CompanyInfo
{
    /// <summary>The company display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The primary market/industry for the company.</summary>
    public string Industry { get; set; } = string.Empty;

    /// <summary>The canonical support email for customer contact.</summary>
    public string SupportEmail { get; set; } = string.Empty;
}

