using Dapper;
using ParcelTracking.Api.Data;
using ParcelTracking.Api.DTOs.PaymentDtos;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DapperContext _context;

        public PaymentRepository(DapperContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var query = @"SELECT PaymentId, CustomerId, ParcelId, Amount, Status, PaymentDate
                          FROM Payments
                          ORDER BY PaymentDate DESC;";
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<PaymentResponseDto>(query);
        }

        public async Task<int> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var query = @"INSERT INTO Payments (CustomerId, ParcelId, Amount, Status)
                          VALUES (@CustomerId, @ParcelId, @Amount, 'Pending');
                          SELECT LAST_INSERT_ID();";
                using var conn =  _context.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(query, dto);
        }

        public async Task<int> UpdatePaymentStatusAsync(int paymentId, string status)
        {
            var query = @"UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { PaymentId = paymentId, Status = status });
        }

        public async Task<int> DeletePaymentAsync(int paymentId)
        {
            var query = @"DELETE FROM Payments WHERE PaymentId = @PaymentId";
            using var conn = _context.CreateConnection();
            return await conn.ExecuteAsync(query, new { PaymentId = paymentId });
        }
    }
}
