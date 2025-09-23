using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.CourierDTOs;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParcelTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class CourierController : ControllerBase
    {
        private readonly ICourierRepository _repo;

        public CourierController(ICourierRepository repo)
        {
            _repo = repo;
        }

        // ✅ Get all couriers (Admin & Customer)
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _repo.GetAllCouriersAsync();
            if (couriers == null || !couriers.Any())
                return NotFound(ApiResponse<string>.FailResponse("No couriers in the database"));

            return Ok(ApiResponse<IEnumerable<CourierResponseDTO>>.SuccessResponse(couriers, "Couriers retrieved successfully"));
        }

        // ✅ Get courier by Id (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourierById(int id)
        {
            var courier = await _repo.GetCourierByIdAsync(id);
            if (courier == null)
                return NotFound(ApiResponse<string>.FailResponse($"No courier found with id: {id}"));

            return Ok(ApiResponse<CourierResponseDTO>.SuccessResponse(courier, "Courier retrieved successfully"));
        }

        // ✅ Add courier (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCourier([FromBody] CreateCourierDTO dto)
        {
            var courierId = await _repo.CreateCourierAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(new { CourierId = courierId }, "Courier added successfully"));
        }

        // ✅ Update courier (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCourier(int id, [FromBody] UpdateCourierDTO dto)
        {
            var rows = await _repo.UpdateCourierAsync(id, dto);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Courier not found or no changes"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Courier updated successfully"));
        }

        // ✅ Delete courier (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourier(int id)
        {
            var rows = await _repo.DeleteCourierAsync(id);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Courier not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Courier deleted successfully"));
        }
    }
}
