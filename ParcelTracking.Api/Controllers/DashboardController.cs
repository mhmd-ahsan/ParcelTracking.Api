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

        public DashboardController(
            ICustomerRepository customerRepo,
            ICourierRepository courierRepo,
            IParcelRepository parcelRepo)
        {
            _customerRepo = customerRepo;
            _courierRepo = courierRepo;
            _parcelRepo = parcelRepo;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var customersCount = await _customerRepo.GetCountAsync();
            var couriersCount = await _courierRepo.GetCountAsync();
            var parcelsCount = await _parcelRepo.GetCountAsync();

            return Ok(new
            {
                totalCustomers = customersCount,
                totalCouriers = couriersCount,
                totalParcels = parcelsCount
            });
        }
    }
}
