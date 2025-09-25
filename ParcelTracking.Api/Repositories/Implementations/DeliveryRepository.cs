using Dapper;
using ParcelTracking.Api.Data;
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

        // Get latest delivery update for a parcel
        public async Task<DeliveryResponseDTO?> GetLatestDeliveryByParcelIdAsync(int parcelId)
        {
            var query = @"
                SELECT *
                FROM Delivery
                WHERE ParcelId = @ParcelId
                ORDER BY UpdatedAt DESC
                LIMIT 1"; // ✅ MySQL-compatible

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DeliveryResponseDTO>(query, new { ParcelId = parcelId });
        }

        // Get full delivery history for a parcel
        public async Task<IEnumerable<DeliveryResponseDTO>> GetDeliveryHistoryByParcelIdAsync(int parcelId)
        {
            var query = @"
                SELECT *
                FROM Delivery
                WHERE ParcelId = @ParcelId
                ORDER BY UpdatedAt DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DeliveryResponseDTO>(query, new { ParcelId = parcelId });
        }

        // Add new delivery update
        public async Task<int> AddDeliveryUpdateAsync(CreateDeliveryDto dto)
        {
            var query = @"
                INSERT INTO Delivery (ParcelId, Status, Location, UpdatedAt)
                VALUES (@ParcelId, @Status, @Location, UTC_TIMESTAMP());
                SELECT LAST_INSERT_ID();"; // ✅ MySQL-compatible

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, dto);
        }

        // Update latest delivery record
        public async Task<bool> UpdateDeliveryStatusAsync(int parcelId, UpdateDeliveryStatusDTO dto)
        {
            var query = @"
                UPDATE Delivery
                SET Status = @Status,
                    Location = COALESCE(@Location, Location), -- ✅ Prevent null crash
                    UpdatedAt = UTC_TIMESTAMP()
                WHERE ParcelId = @ParcelId";

            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, new
            {
                Status = dto.Status,
                Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location,
                ParcelId = parcelId
            });

            return result > 0;
        }

        // Get all deliveries
        public async Task<IEnumerable<DeliveryResponseDTO>> GetAllDeliveriesAsync()
        {
            var query = "SELECT * FROM Delivery ORDER BY UpdatedAt DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DeliveryResponseDTO>(query);
        }

        // Get total delivery count
        public async Task<int> GetCountAsync()
        {
            const string sql = "SELECT COUNT(*) FROM Delivery";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
