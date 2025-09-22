using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.ParcelDtos;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParcelController : ControllerBase
    {
        private readonly IParcelRepository _repo;

        public ParcelController(IParcelRepository repo)
        {
            _repo = repo;
        }

        // ✅ Get all parcels
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllParcels()
        {
            var parcels = await _repo.GetAllParcelsAsync();
            if (parcels == null || !parcels.Any())
            {
                return NotFound(ApiResponse<string>.FailResponse("No parcels in the database"));
            }

            return Ok(ApiResponse<IEnumerable<ParcelResponseDto>>.SuccessResponse(parcels, "Parcels retrieved successfully"));
        }

        // ✅ Get parcel by Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetParcelById(int id)
        {
            var parcel = await _repo.GetParcelByIdAsync(id);
            if (parcel == null)
            {
                return NotFound(ApiResponse<string>.FailResponse($"Parcel with ID {id} not found"));
            }

            return Ok(ApiResponse<ParcelResponseDto>.SuccessResponse(parcel, "Parcel found."));
        }

        // ✅ Track by Tracking Number
        [HttpGet("track/{trackingNo}")]
        public async Task<IActionResult> TrackParcel(string trackingNo)
        {
            var parcel = await _repo.GetParcelByTrackingNoAsync(trackingNo);
            if (parcel == null)
            {
                return NotFound(ApiResponse<string>.FailResponse($"No parcel found with tracking no: {trackingNo}"));
            }

            return Ok(ApiResponse<ParcelResponseDto>.SuccessResponse(parcel, "Parcel tracked successfully."));
        }

        // ✅ Create parcel (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateParcel([FromBody] CreateParcelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid parcel data."));

            var (parcelId, trackingNo) = await _repo.CreateParcelAsync(dto);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { ParcelId = parcelId, TrackingNo = trackingNo },
                "Parcel created successfully"
            ));
        }


        // ✅ Update parcel status (Admin, Courier)
        [Authorize(Roles = "Admin,Courier")]
        [HttpPut("update-status/{id:int}")]
        public async Task<IActionResult> UpdateParcelStatus(int id, [FromBody] UpdateParcelStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid status update data."));

            var rowsAffected = await _repo.UpdateParcelStatusAsync(id, dto);
            if (rowsAffected == 0)
            {
                return NotFound(ApiResponse<string>.FailResponse("Parcel not found or no changes."));
            }

            return Ok(ApiResponse<string>.SuccessResponse(null, "Parcel status updated successfully"));
        }

        // ✅ Delete parcel (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteParcel(int id)
        {
            var rows = await _repo.DeleteParcelAsync(id);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Parcel not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Parcel deleted successfully"));
        }
    }
}
