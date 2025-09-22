using MySql.Data.MySqlClient;
using System.Data;

namespace ParcelTracking.Api.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
