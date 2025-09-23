using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.CustomerDTOs;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Repositories.Interfaces;

namespace ParcelTracking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // ✅ Get all customers (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return Ok(ApiResponse<IEnumerable<CustomerResponseDTO>>.SuccessResponse(customers, "Customers retrieved successfully"));
        }

        // ✅ Get single customer by ID (Admin & Customer themselves)
        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            // Optional: If role is Customer, ensure they can only get their own record
            if (User.IsInRole("Customer"))
            {
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                if (userId != id)
                    return StatusCode(403, ApiResponse<string>.FailResponse("Access denied"));
            }


            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<string>.FailResponse($"No customer found with this id: {id}"));

            return Ok(ApiResponse<CustomerResponseDTO>.SuccessResponse(customer, "Customer found successfully"));
        }

        // ✅ Add new customer (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CreateCustomerDTO dto)
        {
            var customerId = await _customerRepository.CreateCustomerAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(new { CustomerId = customerId }, "Customer added successfully"));
        }

        // ✅ Update customer (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDTO customerDTO)
        {
            var rows = await _customerRepository.UpdateCustomerAsync(id, customerDTO);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Customer not found or no changes"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Customer updated successfully"));
        }

        // ✅ Delete customer (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var rows = await _customerRepository.DeleteCustomerAsync(id);
            if (rows == 0)
                return NotFound(ApiResponse<string>.FailResponse("Customer not found"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Customer deleted successfully"));
        }
    }
}
