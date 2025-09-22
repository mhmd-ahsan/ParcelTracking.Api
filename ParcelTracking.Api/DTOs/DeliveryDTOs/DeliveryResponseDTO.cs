namespace ParcelTracking.Api.DTOs.DeliveryDTOs
{
    public class DeliveryResponseDTO
    {
        public int ParcelId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
