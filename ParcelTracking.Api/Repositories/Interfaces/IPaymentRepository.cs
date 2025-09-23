using ParcelTracking.Api.DTOs.PaymentDtos;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<int> CreatePaymentAsync(CreatePaymentDto dto);
        Task<int> UpdatePaymentStatusAsync(int paymentId, string status);
        Task<int> DeletePaymentAsync(int paymentId);
    }
}
