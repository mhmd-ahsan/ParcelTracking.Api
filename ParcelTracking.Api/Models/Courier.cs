namespace ParcelTracking.Api.Models
{
    public class Courier
    {
        public int CourierId { get; set; }
        public string CourierName { get; set; } = null!;
        public string VehileNo { get; set; } = null!;
        public string Contact { get; set; } = null!;
    }
}
