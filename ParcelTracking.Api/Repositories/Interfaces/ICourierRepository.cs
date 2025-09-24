using ParcelTracking.Api.DTOs.CourierDTOs;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface ICourierRepository
    {
        Task<IEnumerable<CourierResponseDTO>> GetAllCouriersAsync();
        Task<CourierResponseDTO?> GetCourierByIdAsync(int id);
        Task<int> CreateCourierAsync(CreateCourierDTO dto);
        Task<int> UpdateCourierAsync(int id, UpdateCourierDTO dto);
        Task<int> DeleteCourierAsync(int id);

        // ✅ Dashboard count
        Task<int> GetCountAsync();
    }
}
