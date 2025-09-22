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

        public async Task<IEnumerable<ParcelResponseDto>> GetAllParcelsAsync()
        {
            var query = @"SELECT 
                            p.ParcelId, 
                            p.TrackingNo, 
                            c.CustomerName, 
                            co.CourierName AS CourierName, 
                            p.Status, 
                            (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                            p.CreatedAt
                          FROM Parcels p
                          JOIN Customers c ON p.CustomerId = c.CustomerId
                          JOIN Couriers co ON p.CourierId = co.CourierId";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<ParcelResponseDto>(query);
        }

        public async Task<ParcelResponseDto?> GetParcelByIdAsync(int id)
        {
            var query = @"SELECT 
                            p.ParcelId, 
                            p.TrackingNo, 
                            c.CustomerName, 
                            co.CourierName AS CourierName, 
                            p.Status, 
                            (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                            p.CreatedAt
                          FROM Parcels p
                          JOIN Customers c ON p.CustomerId = c.CustomerId
                          JOIN Couriers co ON p.CourierId = co.CourierId
                          WHERE p.ParcelId = @Id";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<ParcelResponseDto>(query, new { Id = id });
        }

        public async Task<ParcelResponseDto?> GetParcelByTrackingNoAsync(string trackingNo)
        {
            var query = @"SELECT 
                            p.ParcelId, 
                            p.TrackingNo, 
                            c.CustomerName, 
                            co.CourierName AS CourierName, 
                            p.Status, 
                            (CAST(p.Weight AS DECIMAL(10,2)) / 1000) AS WeightKg,
                            p.CreatedAt
                          FROM Parcels p
                          JOIN Customers c ON p.CustomerId = c.CustomerId
                          JOIN Couriers co ON p.CourierId = co.CourierId
                          WHERE p.TrackingNo = @TrackingNo";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<ParcelResponseDto>(query, new { TrackingNo = trackingNo });
        }

        public async Task<(int ParcelId, string TrackingNo)> CreateParcelAsync(CreateParcelDto dto)
        {
            string trackingNo = $"TRK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6)}";

            // Normalize weight -> always store in grams
            decimal weightInGrams = dto.Unit.ToLower() switch
            {
                "kg" => dto.Weight * 1000,
                "g" => dto.Weight,
                _ => throw new ArgumentException("Invalid unit. Use 'kg' or 'g'.")
            };

            var query = @"INSERT INTO Parcels (TrackingNo, CustomerId, CourierId, Status, Weight, CreatedAt)
                          VALUES (@TrackingNo, @CustomerId, @CourierId, 'Pending', @Weight, NOW());
                          SELECT LAST_INSERT_ID();";

            using var conn = _context.CreateConnection();
            var parcelId = await conn.ExecuteScalarAsync<int>(query,
                new { TrackingNo = trackingNo, dto.CustomerId, dto.CourierId, Weight = weightInGrams });

            return (parcelId, trackingNo);
        }

        public async Task<int> UpdateParcelStatusAsync(int id, UpdateParcelStatusDto dto)
        {
            var query = @"UPDATE Parcels SET Status = @Status WHERE ParcelId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id, dto.Status });
        }

        public async Task<int> DeleteParcelAsync(int id)
        {
            var query = @"DELETE FROM Parcels WHERE ParcelId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id });
        }
    }
}
