namespace ParcelTracking.Api.DTOs.PaymentDtos
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int ParcelId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
}
