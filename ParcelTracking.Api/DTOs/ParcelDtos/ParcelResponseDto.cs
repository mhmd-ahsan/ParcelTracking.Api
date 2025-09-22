namespace ParcelTracking.Api.DTOs.ParcelDtos
{
    public class ParcelResponseDto
    {
        public int ParcelId { get; set; }
        public string TrackingNo { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CourierName { get; set; } = null!;
        public string Status { get; set; } = null!;

        // Stored in grams, converted to kg when returning
        public decimal WeightKg { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
