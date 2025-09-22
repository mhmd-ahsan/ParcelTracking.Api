namespace ParcelTracking.Api.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int ParcelId { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        public string Location { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
