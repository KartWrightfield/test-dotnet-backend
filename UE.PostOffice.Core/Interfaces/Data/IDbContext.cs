using UE.PostOffice.Core.Entities;

namespace UE.PostOffice.Core.Interfaces.Data
{
    public interface IDbContext
    {
        IQueryable<Supplier> Suppliers { get; }

        IQueryable<Product> Products { get; }
    }
}
