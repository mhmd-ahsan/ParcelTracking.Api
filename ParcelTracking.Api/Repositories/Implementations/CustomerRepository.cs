using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.DTOs.CustomerDTOs;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllCustomersAsync()
        {
            var query = "SELECT CustomerId, CustomerName, Email, Phone, Address FROM Customers";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<CustomerResponseDTO>(query);
        }

        public async Task<CustomerResponseDTO?> GetCustomerByIdAsync(int id)
        {
            var query = @"SELECT CustomerId, CustomerName, Email, Phone, Address 
                          FROM Customers WHERE CustomerId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CustomerResponseDTO?>(query, new { Id = id });
        }

        public async Task<int> CreateCustomerAsync(CreateCustomerDTO dto)
        {
            var query = @"INSERT INTO Customers (CustomerName, Email, Phone, Address) 
                  VALUES(@CustomerName, @Email, @Phone, @Address);
                  SELECT LAST_INSERT_ID();";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(query, dto);
        }


        public async Task<int> UpdateCustomerAsync(int id, UpdateCustomerDTO dto)
        {
            var query = @"UPDATE Customers 
                          SET CustomerName = @CustomerName, Email = @Email, Phone = @Phone, Address = @Address
                          WHERE CustomerId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { dto.CustomerName, dto.Email, dto.Phone, dto.Address, Id = id });
        }

        public async Task<int> DeleteCustomerAsync(int id)
        {
            var query = @"DELETE FROM Customers WHERE CustomerId = @Id";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { Id = id });
        }

        // ✅ Dashboard count
        public async Task<int> GetCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Customers";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
