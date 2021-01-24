namespace CheckoutPaymentAPI.Models.DTOs
{
    /// <summary>
    /// Represents what the API will return when responding to a process payments request
    /// </summary>
    public class ProcessPaymentResponseDTO
    {
        /// <summary>
        /// Gets or sets a value indicating if the process was a success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets a value identifying the payment
        /// </summary>
        public int? PaymentId { get; set; }
    }
}