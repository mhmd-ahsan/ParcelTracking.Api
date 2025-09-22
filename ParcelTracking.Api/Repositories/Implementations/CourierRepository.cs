using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.DTOs.CourierDTOs;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class CourierRepository : ICourierRepository
    {
        private readonly DapperContext _context;

        public CourierRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourierResponseDTO>> GetAllCouriersAsync()
        {
            var query = "SELECT CourierId, CourierName, VehicleNo, Contact FROM Couriers";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<CourierResponseDTO>(query);
        }

        public async Task<CourierResponseDTO?> GetCourierByIdAsync(int id)
        {
            var query = @"SELECT CourierId, CourierName, VehicleNo, Contact 
                          FROM Couriers WHERE CourierId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CourierResponseDTO>(query, new { Id = id });
        }

        public async Task<int> CreateCourierAsync(CreateCourierDTO dto)
        {
            var query = @"
                INSERT INTO Couriers (CourierName, VehicleNo, Contact)
                VALUES (@CourierName, @VehicleNo, @Contact);
                SELECT LAST_INSERT_ID();";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(query, dto);
        }

        public async Task<int> UpdateCourierAsync(int id, UpdateCourierDTO dto)
        {
            var query = @"
                UPDATE Couriers
                SET CourierName = @CourierName, VehicleNo = @VehicleNo, Contact = @Contact
                WHERE CourierId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { dto.CourierName, dto.VehicleNo, dto.Contact, Id = id });
        }

        public async Task<int> DeleteCourierAsync(int id)
        {
            var query = "DELETE FROM Couriers WHERE CourierId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id });
        }
    }
}
