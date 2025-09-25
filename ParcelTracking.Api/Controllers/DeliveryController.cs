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

        // ✅ Get the latest delivery update for a parcel
        // GET: api/delivery/latest/{parcelId}
        [HttpGet("latest/{parcelId}")]
        public async Task<IActionResult> GetLatestDelivery(int parcelId)
        {
            var delivery = await _deliveryRepo.GetLatestDeliveryByParcelIdAsync(parcelId);
            if (delivery == null)
                return NotFound(new { message = "No delivery record found for this parcel" });

            return Ok(delivery);
        }

        // ✅ Get full delivery history of a parcel
        // GET: api/delivery/history/{parcelId}
        [HttpGet("history/{parcelId}")]
        public async Task<IActionResult> GetDeliveryHistory(int parcelId)
        {
            var history = await _deliveryRepo.GetDeliveryHistoryByParcelIdAsync(parcelId);
            if (history == null || !history.Any())
                return NotFound(new { message = "No delivery history found for this parcel" });

            return Ok(history);
        }

        // ✅ Add a new delivery update (real-world accurate)
        // POST: api/delivery/add
        [HttpPost("add")]
        public async Task<IActionResult> AddDeliveryUpdate(CreateDeliveryDto dto)
        {
            var id = await _deliveryRepo.AddDeliveryUpdateAsync(dto);
            if (id <= 0)
                return BadRequest(new { message = "Failed to add delivery update" });

            return Ok(new { message = "Delivery update added successfully", deliveryId = id });
        }

        // ⚡ Optional: Update delivery directly (less real-world, but still useful for admin/testing)
        // PUT: api/delivery/update/{parcelId}
        [HttpPut("update/{parcelId}")]
        public async Task<IActionResult> UpdateStatus(int parcelId, UpdateDeliveryStatusDTO dto)
        {
            var updated = await _deliveryRepo.UpdateDeliveryStatusAsync(parcelId, dto);
            if (!updated)
                return BadRequest(new { message = "Update failed" });

            return Ok(new { message = "Delivery status updated successfully" });
        }

        // ✅ Get all deliveries (for admin dashboard)
        // GET: api/delivery
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var deliveries = await _deliveryRepo.GetAllDeliveriesAsync();
            return Ok(deliveries);
        }
    }
}
