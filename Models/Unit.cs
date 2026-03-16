namespace SalesApiStub.Models;

/// <summary>
/// Represents the unit used for pricing and stock quantities of a product.
/// </summary>
public enum Unit
{
    /// <summary>Sold per individual item.</summary>
    PerPiece,

    /// <summary>Sold per kilogram.</summary>
    PerKilogram,

    /// <summary>Sold per meter.</summary>
    PerMeter,

    /// <summary>Sold per ton.</summary>
    PerTon
}