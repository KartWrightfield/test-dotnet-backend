namespace UE.PostOffice.Api.Model
{
    using System;

    /// <summary>
    /// Represents a despatch date for an order in the post office system.
    /// </summary>
    public class DespatchDateResponse
    {
        /// <summary>
        /// The datetime representation of when the order will be despatched
        /// </summary>
        public DateTime Date { get; init; }
    }
}