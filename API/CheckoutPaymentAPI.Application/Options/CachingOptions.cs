namespace CheckoutPaymentAPI.Application.Options
{
    /// <summary>
    /// Represents caching options to be used in the application
    /// </summary>
    public class CachingOptions
    {
        public const string Key = "Caching";

        /// <summary>
        /// Gets or sets the TTL in minutes that processed payments should have when added to a cache
        /// </summary>
        public int ProccessedPaymentTTLMinutes { get; set; }
            = 1;
    }
}
