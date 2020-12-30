using IdentityModel;
using IdentityServer4.Validation;
using IdentityServer4Example.STS.Models;
using IdentityServer4Example.STS.Repositories;
using System.Threading.Tasks;

namespace IdentityServer4Example.STS.IdentityServer4.Validators
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;

        public ResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (await _userRepository.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                User user = await _userRepository.GetUserByUsernameAsync(context.UserName);
                context.Result = new GrantValidationResult(user.UserId.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}