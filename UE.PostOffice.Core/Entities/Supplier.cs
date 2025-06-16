namespace UE.PostOffice.Core.Entities
{
    /// <summary>
    /// Represents a supplier in the Post Office system.
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// The unique identifer for the supplier.
        /// </summary>
        public int SupplierId { get; set; }

        /// <summary>
        /// The name of the supplier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The lead time of the supplier (i.e. the number of days they require to get any of their products to the
        /// post office for despatch).
        /// </summary>
        public int LeadTime { get; set; }
    }
}
