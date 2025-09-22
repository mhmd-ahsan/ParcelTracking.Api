namespace ParcelTracking.Api.DTOs.CourierDTOs
{
    public class CourierResponseDTO
    {
        public int CourierId { get; set; }
        public string CourierName { get; set; } = null!;
        public string VehicleNo { get; set; } = null!;
        public string Contact { get; set; } = null!;
    }
}
