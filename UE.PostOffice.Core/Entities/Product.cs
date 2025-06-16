namespace UE.PostOffice.Core.Entities
{
    /// <summary>
    /// Represents a product in the post office system.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The unique identifier for the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the supplier that supplies this product
        /// </summary>
        public int SupplierId { get; set; }
    }
}
