using IdentityServer4Example.STS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4Example.STS.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();

        Task<User> GetUserByIdAsync(int userId);

        Task<User> GetUserByUsernameAsync(string username);

        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}