namespace UE.PostOffice.Core.Interfaces.Repositories;

public interface ISupplierRepository
{
    Task<int> GetMaxLeadTimeForProducts(List<int> productIds);
}