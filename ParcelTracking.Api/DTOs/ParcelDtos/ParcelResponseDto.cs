public class DeliveryDto
{
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class ParcelResponseDto
{
    public int ParcelId { get; set; }
    public string TrackingNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CourierName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public DateTime CreatedAt { get; set; }

    public DeliveryDto? Delivery { get; set; } // Add Delivery info here
}
