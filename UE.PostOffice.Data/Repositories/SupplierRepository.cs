using System.Collections.Generic;
using System.Linq;
using UE.PostOffice.Core.Interfaces.Repositories;

namespace UE.PostOffice.Data.Repositories;

public class SupplierRepository(IDbContext dbContext) : ISupplierRepository
{
    public int GetMaxLeadTimeForProducts(List<int> productIds)
    {
        return dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => p.SupplierId)
            .Distinct()
            .Join(dbContext.Suppliers,
                supplierId => supplierId,
                supplier => supplier.SupplierId,
                (_, supplier) => supplier.LeadTime)
            .DefaultIfEmpty(0)
            .Max();
    }
}