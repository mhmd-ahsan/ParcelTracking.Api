namespace ParcelTracking.Api.DTOs.CustomerDTOs
{
    public class UpdateCustomerDTO
    {
        public string CustomerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
