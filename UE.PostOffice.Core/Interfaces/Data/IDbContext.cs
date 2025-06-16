using UE.PostOffice.Core.Entities;

namespace UE.PostOffice.Core.Interfaces.Data
{
    /// <summary>
    /// Represents the contract for a database context that provides access to data entities within the system.
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Gets the collection of supplier entities available within the database context.
        /// </summary>
        IQueryable<Supplier> Suppliers { get; }

        /// <summary>
        /// Gets the collection of product entities available within the database context.
        /// </summary>
        IQueryable<Product> Products { get; }
    }
}
