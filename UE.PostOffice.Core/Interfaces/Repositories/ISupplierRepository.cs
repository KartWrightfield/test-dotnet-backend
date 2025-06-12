namespace UE.PostOffice.Core.Interfaces.Repositories;

public interface ISupplierRepository
{
    int GetMaxLeadTimeForProducts(List<int> productIds);
}