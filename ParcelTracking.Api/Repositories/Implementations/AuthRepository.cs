using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.Models;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;

        public AuthRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var query = "SELECT * FROM Users WHERE Email = @Email";
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }

        public async Task<int?> RegisterUserAsync(User user)
        {
            var query = @"INSERT INTO Users (Name, Email, PasswordHash, Role)
                  VALUES (@Name, @Email, @PasswordHash, @Role);
                  SELECT LAST_INSERT_ID();"; // ✅ MySQL version

            using var conn = _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(query, user);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            using var conn = _context.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }
    }
}
