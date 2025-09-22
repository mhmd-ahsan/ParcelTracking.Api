namespace ParcelTracking.Api.DTOs.ParcelDtos
{
    public class CreateParcelDto
    {
        public int CustomerId { get; set; }
        public int CourierId { get; set; }


        // user provides weight value
        public decimal Weight { get; set; }

        // user specifies unit: "g" or "kg"
        public string Unit { get; set; } = "kg";
    }
}
