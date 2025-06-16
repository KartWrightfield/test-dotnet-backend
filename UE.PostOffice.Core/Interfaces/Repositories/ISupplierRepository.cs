namespace UE.PostOffice.Core.Interfaces.Repositories;

/// <summary>
/// Represents a contract for accessing supplier-related data and operations.
/// </summary>
public interface ISupplierRepository
{
    /// <summary>
    /// Retrieves the maximum lead time for the specified products based on their associated suppliers.
    /// </summary>
    /// <param name="productIds">A list of product IDs to determine the maximum lead time for.</param>
    /// <returns>The maximum lead time, in days, among the supplied products. Returns 0 if no matching products are found.</returns>
    Task<int> GetMaxLeadTimeForProducts(List<int> productIds);
}