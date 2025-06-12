using System.Collections.Generic;
using UE.PostOffice.Core.Interfaces.Repositories;

namespace UE.PostOffice.Data.Repositories;

public class SupplierRepository : ISupplierRepository
{
    public int GetMaxLeadTimeForProducts(List<int> productIds)
    {
        throw new System.NotImplementedException();
    }
}