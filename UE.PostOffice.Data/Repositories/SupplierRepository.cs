using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UE.PostOffice.Core.Interfaces.Data;
using UE.PostOffice.Core.Interfaces.Repositories;

namespace UE.PostOffice.Data.Repositories;

/// <summary>
/// Provides data access methods for supplier-related operations, implemented from ISupplierRepository.
/// This repository communicates with the underlying data context to perform queries related to suppliers.
/// </summary>
public class SupplierRepository(IDbContext dbContext) : ISupplierRepository
{
    /// <summary>
    /// Retrieves the maximum lead time among suppliers for the given list of product IDs.
    /// </summary>
    /// <param name="productIds">A list of product IDs to find the maximum lead time for their associated suppliers.</param>
    /// <returns>The maximum lead time among the suppliers of the specified products. If no suppliers are found, returns 0.</returns>
    public Task<int> GetMaxLeadTimeForProducts(List<int> productIds) 
    {
        var result = dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => p.SupplierId)
            .Distinct()
            .Join(dbContext.Suppliers,
                supplierId => supplierId,
                supplier => supplier.SupplierId,
                (_, supplier) => supplier.LeadTime)
            .DefaultIfEmpty(0)
            .Max(); //Would be MaxAsync in an actual async method if we were using EF and a real database 

        return Task.FromResult(result);
    }
}