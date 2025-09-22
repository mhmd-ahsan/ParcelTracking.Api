using ParcelTracking.Api.Models;

namespace ParcelTracking.Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<int?> RegisterUserAsync(User user);
        Task<bool> UserExistsAsync(string email);
    }
}
