using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UE.PostOffice.Core.Interfaces.Data;
using UE.PostOffice.Core.Interfaces.Repositories;

namespace UE.PostOffice.Data.Repositories;

public class SupplierRepository(IDbContext dbContext) : ISupplierRepository
{
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