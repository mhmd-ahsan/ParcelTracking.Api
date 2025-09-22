using Microsoft.AspNetCore.Mvc;
using ParcelTracking.Api.DTOs.Auth;
using ParcelTracking.Api.Helpers;
using ParcelTracking.Api.Models;
using ParcelTracking.Api.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ParcelTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IAuthRepository authRepository, JwtHelper jwtHelper)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (await _authRepository.UserExistsAsync(dto.Email))
                return BadRequest(ApiResponse<string>.FailResponse("User with this email already exists."));

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), // ✅ inline hash
                Role = "User"
            };

            var userId = await _authRepository.RegisterUserAsync(user);

            if (userId == null)
                return StatusCode(500, ApiResponse<string>.FailResponse("User registration failed."));

            return Ok(ApiResponse<string>.SuccessResponse("User registered successfully"));
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(ApiResponse<string>.FailResponse("Invalid email or password."));

            var token = _jwtHelper.GenerateToken(user.Id.ToString(), user.Role);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));

        }
    }
}
