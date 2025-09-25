using ParcelTracking.Api.DTOs.DeliveryDTOs;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        // ✅ Get only the latest delivery update for a parcel
        Task<DeliveryResponseDTO?> GetLatestDeliveryByParcelIdAsync(int parcelId);

        // ✅ Get full delivery history for a parcel
        Task<IEnumerable<DeliveryResponseDTO>> GetDeliveryHistoryByParcelIdAsync(int parcelId);

        // ✅ Add new delivery update (instead of overwriting)
        Task<int> AddDeliveryUpdateAsync(CreateDeliveryDto dto);

        // ✅ Update delivery status directly (optional, less real-world accurate)
        Task<bool> UpdateDeliveryStatusAsync(int parcelId, UpdateDeliveryStatusDTO dto);

        // ✅ Get all deliveries (for admin dashboard)
        Task<IEnumerable<DeliveryResponseDTO>> GetAllDeliveriesAsync();

        Task<int> GetCountAsync();

    }
}
