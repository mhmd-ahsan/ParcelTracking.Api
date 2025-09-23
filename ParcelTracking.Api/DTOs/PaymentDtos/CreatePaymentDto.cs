namespace ParcelTracking.Api.DTOs.PaymentDtos
{
    public class CreatePaymentDto
    {
        public int CustomerId { get; set; }
        public int ParcelId { get; set; }
        public decimal Amount { get; set; }
    }
}
