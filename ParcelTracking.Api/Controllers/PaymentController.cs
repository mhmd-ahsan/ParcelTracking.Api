using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.PaymentDtos;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParcelTracking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _repo;

        public PaymentController(IPaymentRepository repo)
        {
            _repo = repo;
        }

        // ✅ Get All Payments (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _repo.GetAllPaymentsAsync();
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.SuccessResponse(payments, "Payments retrieved successfully"));
        }

        // ✅ Create Payment (Admin & Customer)
        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid payment data."));

            var paymentId = await _repo.CreatePaymentAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(new { PaymentId = paymentId }, "Payment created successfully"));
        }

        // ✅ Update Payment Status (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("update-status/{id:int}")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string status)
        {
            var rows = await _repo.UpdatePaymentStatusAsync(id, status);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Payment not found or no changes."));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Payment status updated successfully"));
        }

        // ✅ Delete Payment (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var rows = await _repo.DeletePaymentAsync(id);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Payment not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Payment deleted successfully"));
        }
    }
}
