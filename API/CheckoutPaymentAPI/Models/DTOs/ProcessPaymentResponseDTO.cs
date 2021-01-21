namespace CheckoutPaymentAPI.Models.DTOs
{
    public class ProcessPaymentResponseDTO
    {
        public bool Success { get; set; }
        public int? PaymentId { get; set; }
    }
}