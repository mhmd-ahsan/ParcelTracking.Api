using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.Models;
using ParcelTracking.Api.DTOs.DeliveryDTOs;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DapperContext _context;

        public DeliveryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DeliveryResponseDTO?> GetDeliveryByParcelIdAsync(int parcelId)
        {
            var query = "SELECT * FROM Delivery WHERE ParcelId = @ParcelId";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<DeliveryResponseDTO>(query, new { ParcelId = parcelId });
        }

        public async Task<bool> UpdateDeliveryStatusAsync(int parcelId, UpdateDeliveryStatusDTO dto)
        {
            var query = @"
                UPDATE Delivery
                SET Status = @Status,
                    Location = @Location,
                    UpdatedAt = @UpdatedAt
                WHERE ParcelId = @ParcelId";

            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, new { dto.Status, dto.Location, UpdatedAt = DateTime.UtcNow, ParcelId = parcelId });
            return result > 0;
        }

        public async Task<IEnumerable<DeliveryResponseDTO>> GetAllDeliveriesAsync()
        {
            var query = "SELECT * FROM Delivery";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DeliveryResponseDTO>(query);
        }
    }
}
