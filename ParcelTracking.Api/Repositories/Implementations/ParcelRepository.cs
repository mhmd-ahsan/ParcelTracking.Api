using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.DTOs.ParcelDtos;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class ParcelRepository : IParcelRepository
    {
        private readonly DapperContext _context;

        public ParcelRepository(DapperContext context)
        {
            _context = context;
        }

        // ✅ Get all parcels with their latest delivery
        public async Task<IEnumerable<ParcelResponseDto>> GetAllParcelsAsync()
        {
            var query = @"
                SELECT 
                    p.ParcelId, 
                    p.TrackingNo, 
                    c.CustomerName, 
                    co.CourierName AS CourierName, 
                    p.Status, 
                    (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                    p.CreatedAt,
                    d.Status AS DeliveryStatus,
                    d.Location AS DeliveryLocation,
                    d.UpdatedAt AS DeliveryUpdatedAt
                FROM Parcels p
                JOIN Customers c ON p.CustomerId = c.CustomerId
                JOIN Couriers co ON p.CourierId = co.CourierId
                LEFT JOIN Delivery d ON d.ParcelId = p.ParcelId
                WHERE d.UpdatedAt = (
                    SELECT MAX(dd.UpdatedAt)
                    FROM Delivery dd
                    WHERE dd.ParcelId = p.ParcelId
                )
                OR d.UpdatedAt IS NULL;";

            using var conn = _context.CreateConnection();
            var results = await conn.QueryAsync(query);

            return results.Select(r => new ParcelResponseDto
            {
                ParcelId = r.ParcelId,
                TrackingNo = r.TrackingNo,
                CustomerName = r.CustomerName,
                CourierName = r.CourierName,
                Status = r.Status,
                WeightKg = r.WeightKg,
                CreatedAt = r.CreatedAt,
                Delivery = r.DeliveryStatus != null ? new DeliveryDto
                {
                    Status = r.DeliveryStatus,
                    Location = r.DeliveryLocation,
                    UpdatedAt = r.DeliveryUpdatedAt
                } : null
            });
        }

        // ✅ Get parcel by ID with latest delivery
        public async Task<ParcelResponseDto?> GetParcelByIdAsync(int id)
        {
            var query = @"
                SELECT 
                    p.ParcelId, 
                    p.TrackingNo, 
                    c.CustomerName, 
                    co.CourierName AS CourierName, 
                    p.Status, 
                    (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                    p.CreatedAt,
                    d.Status AS DeliveryStatus,
                    d.Location AS DeliveryLocation,
                    d.UpdatedAt AS DeliveryUpdatedAt
                FROM Parcels p
                JOIN Customers c ON p.CustomerId = c.CustomerId
                JOIN Couriers co ON p.CourierId = co.CourierId
                LEFT JOIN Delivery d ON d.ParcelId = p.ParcelId
                WHERE p.ParcelId = @Id
                  AND (d.UpdatedAt = (
                      SELECT MAX(dd.UpdatedAt)
                      FROM Delivery dd
                      WHERE dd.ParcelId = p.ParcelId
                  ) OR d.UpdatedAt IS NULL);";

            using var conn = _context.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync(query, new { Id = id });

            if (result == null) return null;

            return new ParcelResponseDto
            {
                ParcelId = result.ParcelId,
                TrackingNo = result.TrackingNo,
                CustomerName = result.CustomerName,
                CourierName = result.CourierName,
                Status = result.Status,
                WeightKg = result.WeightKg,
                CreatedAt = result.CreatedAt,
                Delivery = result.DeliveryStatus != null ? new DeliveryDto
                {
                    Status = result.DeliveryStatus,
                    Location = result.DeliveryLocation,
                    UpdatedAt = result.DeliveryUpdatedAt
                } : null
            };
        }

        // ✅ Get parcel by TrackingNo with latest delivery
        public async Task<ParcelResponseDto?> GetParcelByTrackingNoAsync(string trackingNo)
        {
            var query = @"
                SELECT 
                    p.ParcelId, 
                    p.TrackingNo, 
                    c.CustomerName, 
                    co.CourierName AS CourierName, 
                    p.Status, 
                    (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                    p.CreatedAt,
                    d.Status AS DeliveryStatus,
                    d.Location AS DeliveryLocation,
                    d.UpdatedAt AS DeliveryUpdatedAt
                FROM Parcels p
                JOIN Customers c ON p.CustomerId = c.CustomerId
                JOIN Couriers co ON p.CourierId = co.CourierId
                LEFT JOIN Delivery d ON d.ParcelId = p.ParcelId
                WHERE p.TrackingNo = @TrackingNo
                  AND (d.UpdatedAt = (
                      SELECT MAX(dd.UpdatedAt)
                      FROM Delivery dd
                      WHERE dd.ParcelId = p.ParcelId
                  ) OR d.UpdatedAt IS NULL);";

            using var conn = _context.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync(query, new { TrackingNo = trackingNo });

            if (result == null) return null;

            return new ParcelResponseDto
            {
                ParcelId = result.ParcelId,
                TrackingNo = result.TrackingNo,
                CustomerName = result.CustomerName,
                CourierName = result.CourierName,
                Status = result.Status,
                WeightKg = result.WeightKg,
                CreatedAt = result.CreatedAt,
                Delivery = result.DeliveryStatus != null ? new DeliveryDto
                {
                    Status = result.DeliveryStatus,
                    Location = result.DeliveryLocation,
                    UpdatedAt = result.DeliveryUpdatedAt
                } : null
            };
        }

        // ✅ Create parcel + default delivery entry
        public async Task<(int ParcelId, string TrackingNo)> CreateParcelAsync(CreateParcelDto dto)
        {
            string trackingNo = $"TRK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6)}";

            decimal weightInGrams = dto.Unit.ToLower() switch
            {
                "kg" => dto.Weight * 1000,
                "g" => dto.Weight,
                _ => throw new ArgumentException("Invalid unit. Use 'kg' or 'g'.")
            };

            var insertParcelQuery = @"INSERT INTO Parcels (TrackingNo, CustomerId, CourierId, Status, Weight, CreatedAt)
                              VALUES (@TrackingNo, @CustomerId, @CourierId, 'Pending', @Weight, NOW());
                              SELECT LAST_INSERT_ID();";

            using var conn = _context.CreateConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                var parcelId = await conn.ExecuteScalarAsync<int>(insertParcelQuery,
                    new { TrackingNo = trackingNo, dto.CustomerId, dto.CourierId, Weight = weightInGrams },
                    transaction);

                var insertDeliveryQuery = @"INSERT INTO Delivery (ParcelId, Status, Location, UpdatedAt)
                                    VALUES (@ParcelId, 'Pending', '', NOW());";

                await conn.ExecuteAsync(insertDeliveryQuery, new { ParcelId = parcelId }, transaction);

                transaction.Commit();

                return (parcelId, trackingNo);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ✅ Update status
        public async Task<int> UpdateParcelStatusAsync(int id, UpdateParcelStatusDto dto)
        {
            var query = @"UPDATE Parcels SET Status = @Status WHERE ParcelId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id, dto.Status });
        }

        // ✅ Delete parcel
        public async Task<int> DeleteParcelAsync(int id)
        {
            var query = @"DELETE FROM Parcels WHERE ParcelId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id });
        }

        // ✅ Dashboard count
        public async Task<int> GetCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Parcels";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
