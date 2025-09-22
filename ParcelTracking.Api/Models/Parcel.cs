namespace ParcelTracking.Api.Models
{
    public class Parcel
    {
        public int ParcelId { get; set; }
        public string TrackingNo { get; set; } = null!;
        public int CustomerId { get; set; }
        public int CourierId { get; set; }
        public string Status { get; set; } = "Pending"; // Default: Pending
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
