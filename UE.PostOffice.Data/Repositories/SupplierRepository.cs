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
            .Join(dbContext.Suppliers,
                product => product.SupplierId,
                supplier => supplier.SupplierId,
                (product, supplier) => supplier.LeadTime)
            .Max();
    }
}