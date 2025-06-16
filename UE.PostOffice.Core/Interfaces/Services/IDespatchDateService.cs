namespace UE.PostOffice.Core.Interfaces.Services;

/// <summary>
/// Interface for a service responsible for calculating the despatch date of an order based on products and date.
/// </summary>
public interface IDespatchDateService
{
    /// <summary>
    /// Calculates the despatch date for an order based on the provided product IDs and order date.
    /// </summary>
    /// <param name="productIds">A list of IDs representing the products in the order.</param>
    /// <param name="orderDate">The date on which the order was placed.</param>
    /// <returns>The calculated despatch date.</returns>
    Task<DateTime> CalculateDespatchDate(List<int> productIds, DateTime orderDate);
}