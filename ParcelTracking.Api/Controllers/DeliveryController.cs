using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.DeliveryDTOs;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryRepository _deliveryRepo;

        public DeliveryController(IDeliveryRepository deliveryRepo)
        {
            _deliveryRepo = deliveryRepo;
        }

        // GET: api/delivery/track/{parcelId}
        [HttpGet("track/{parcelId}")]
        public async Task<IActionResult> TrackParcel(int parcelId)
        {
            var delivery = await _deliveryRepo.GetDeliveryByParcelIdAsync(parcelId);
            if (delivery == null)
                return NotFound(new { message = "Parcel not found" });

            return Ok(delivery);
        }

        // PUT: api/delivery/update/{parcelId}
        [HttpPut("update/{parcelId}")]
        public async Task<IActionResult> UpdateStatus(int parcelId, UpdateDeliveryStatusDTO dto)
        {
            var updated = await _deliveryRepo.UpdateDeliveryStatusAsync(parcelId, dto);
            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Delivery status updated successfully" });
        }
    }
}
