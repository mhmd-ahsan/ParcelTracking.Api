using ParcelTracking.Api.DTOs.ParcelDtos;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface IParcelRepository
    {
        Task<IEnumerable<ParcelResponseDto>> GetAllParcelsAsync();
        Task<ParcelResponseDto?> GetParcelByIdAsync(int id);
        Task<(int ParcelId, string TrackingNo)> CreateParcelAsync(CreateParcelDto dto);
        Task<int> UpdateParcelStatusAsync(int id, UpdateParcelStatusDto dto);
        Task<int> DeleteParcelAsync(int id);
        Task<ParcelResponseDto?> GetParcelByTrackingNoAsync(string trackingNo);
    }
}
