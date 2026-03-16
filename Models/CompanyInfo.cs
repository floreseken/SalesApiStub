namespace SalesApiStub.Models;

/// <summary>
/// Represents basic company profile details.
/// </summary>
public class CompanyInfo
{
    /// <summary>The company display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The primary market/industry for the company.</summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>The canonical support email for customer contact.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The culture of the contact person.  </summary>
    public string Culture { get; set; } = "nl-NL";
}