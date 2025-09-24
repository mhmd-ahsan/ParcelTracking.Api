using ParcelTracking.Api.DTOs.CustomerDTOs;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerResponseDTO>> GetAllCustomersAsync();
        Task<CustomerResponseDTO?> GetCustomerByIdAsync(int id);
        Task<int> CreateCustomerAsync(CreateCustomerDTO dTO);
        Task<int> UpdateCustomerAsync(int id, UpdateCustomerDTO dTO);
        Task<int> DeleteCustomerAsync(int id);

        // ✅ Dashboard count
        Task<int> GetCountAsync();
    }
}
