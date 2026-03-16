namespace SalesApiStub.Models;

/// <summary>
/// Represents a known company address.
/// </summary>
public class Address
{
    /// <summary>A stable identifier for the address.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>The address label for quick identification (for example, HQ or Warehouse).</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>The first street line of the address.</summary>
    public string Line1 { get; set; } = string.Empty;

    /// <summary>The city or locality.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>The postal or ZIP code.</summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>The country code or country name.</summary>
    public string Country { get; set; } = string.Empty;
}

