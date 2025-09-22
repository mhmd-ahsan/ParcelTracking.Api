using ParcelTracking.Api.Models;
using ParcelTracking.Api.DTOs.DeliveryDTOs;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<DeliveryResponseDTO?> GetDeliveryByParcelIdAsync(int parcelId);
        Task<bool> UpdateDeliveryStatusAsync(int parcelId, UpdateDeliveryStatusDTO dto);
        Task<IEnumerable<DeliveryResponseDTO>> GetAllDeliveriesAsync();
    }
}
