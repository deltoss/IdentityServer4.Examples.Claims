using IdentityServer4Example.STS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4Example.STS.Repositories
{
    /// <summary>
    /// Simulates managing users from the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        public Task<List<User>> GetUsersAsync()
        {
            return Task.FromResult(new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    Username = "scott",
                    Password = "password",
                    Email = "scott@scottbrady91.com",
                    Role = "admin",
                    IsActive = true,
                }
            });
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return (await GetUsersAsync()).FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return (await GetUsersAsync()).FirstOrDefault(x => x.Username == username);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            return (await GetUsersAsync()).Any(x => x.Username == username && x.Password == password);
        }
    }
}