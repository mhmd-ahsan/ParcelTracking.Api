using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ParcelTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICourierRepository _courierRepo;
        private readonly IParcelRepository _parcelRepo;
        private readonly IDeliveryRepository _deliveryRepo;

        public DashboardController(
            ICustomerRepository customerRepo,
            ICourierRepository courierRepo,
            IParcelRepository parcelRepo,
            IDeliveryRepository deliveryRepo)
        {
            _customerRepo = customerRepo;
            _courierRepo = courierRepo;
            _parcelRepo = parcelRepo;
            _deliveryRepo = deliveryRepo;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var customersCount = await _customerRepo.GetCountAsync();
            var couriersCount = await _courierRepo.GetCountAsync();
            var parcelsCount = await _parcelRepo.GetCountAsync();
            var deliveriesCount = await _deliveryRepo.GetCountAsync();

            return Ok(new
            {
                totalCustomers = customersCount,
                totalCouriers = couriersCount,
                totalParcels = parcelsCount,
                totalDeliveries = deliveriesCount
            });
        }
    }
}
