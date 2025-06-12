using System.Linq;

namespace UE.PostOffice.Data
{
    public interface IDbContext
    {
        IQueryable<Supplier> Suppliers { get; }

        IQueryable<Product> Products { get; }
    }
}
