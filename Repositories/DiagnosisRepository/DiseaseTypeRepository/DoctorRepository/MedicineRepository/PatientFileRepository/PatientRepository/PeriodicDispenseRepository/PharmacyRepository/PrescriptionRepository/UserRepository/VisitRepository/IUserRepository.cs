using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tadawi.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password, string role);
        Task<bool> DeleteUserAsync(string userId);
    }
}