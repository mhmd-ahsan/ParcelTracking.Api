using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.CourierDTOs;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Models;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParcelTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly ICourierRepository _repo;

        public CourierController(ICourierRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _repo.GetAllCouriersAsync();
            if (couriers == null || !couriers.Any())
            {
                return NotFound(ApiResponse<string>.FailResponse("No couriers in the database"));
            }
            return Ok(ApiResponse<IEnumerable<CourierResponseDTO>>.SuccessResponse(couriers, "Couriers retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourierById(int id)
        {
            var courier = await _repo.GetCourierByIdAsync(id);
            if (courier == null)
                return NotFound(ApiResponse<string>.FailResponse($"No courier found with id: {id}"));

            return Ok(ApiResponse<CourierResponseDTO>.SuccessResponse(courier, "Courier retrieved successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCourier([FromBody] CreateCourierDTO dto)
        {
            var courierId = await _repo.CreateCourierAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(new { CourierId = courierId }, "Courier added successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourier(int id, [FromBody] UpdateCourierDTO dto)
        {
            var rows = await _repo.UpdateCourierAsync(id, dto);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Courier not found or no changes"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Courier updated successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourier(int id)
        {
            var rows = await _repo.DeleteCourierAsync(id);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Courier not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Courier deleted successfully"));
        }
    }
}
