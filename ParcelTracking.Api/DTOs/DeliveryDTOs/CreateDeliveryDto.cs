namespace ParcelTracking.Api.DTOs.DeliveryDTOs
{
    public class CreateDeliveryDto
    {
        public int ParcelId { get; set; }
        public string Status { get; set; } = null!;
    }
}
