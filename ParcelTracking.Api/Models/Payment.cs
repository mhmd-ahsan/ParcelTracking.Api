using System;

namespace ParcelTracking.Api.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int ParcelId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
