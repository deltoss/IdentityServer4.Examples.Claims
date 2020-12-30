using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4Example.STS.Models;
using IdentityServer4Example.STS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4Example.STS.IdentityServer4.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region IProfileService Implementation

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // If an access token, add claims requested via user claims defined as part of API Resources.
            // If an identity token, add claims requested via user claims defined as part of Identity Resources.
            context.AddRequestedClaims(context.Subject.Claims);

            List<string> scopes = context.RequestedResources.ParsedScopes.Select(p => p.ParsedName).ToList();
            IEnumerable<Claim> claimsToAddManually = await GetClaimsAsync(scopes, context);
            
            // Add to IssuedClaims so that it'd be added as part of the access token or identity token.
            context.IssuedClaims.AddRange(claimsToAddManually);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            if (subjectId != null)
            {
                User user = await GetSubjectUserAsync(subjectId);
                context.IsActive = user.IsActive;
            }
            else
            {
                context.IsActive = false;
            }
        }

        #endregion IProfileService Implementation

        #region Private Methods

        private async Task<User> GetSubjectUserAsync(string subjectId)
        {
            int userId = int.TryParse(subjectId, out userId) ? userId : 0;
            if (userId == 0)
            {
                throw new InvalidCastException($"Could not convert '{subjectId}' to a valid int.");
            }

            User user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with the ID of '{subjectId}' is not found.");
            }

            return user;
        }

        private async Task<List<Claim>> GetClaimsAsync(IEnumerable<string> scopes, ProfileDataRequestContext context)
        {
            User user = await GetSubjectUserAsync(context.Subject.GetSubjectId());

            var claims = new List<Claim>();

            foreach (var scope in scopes)
            {
                claims.AddRange(GetClaims(user, scope));
            }

            return claims;
        }

        private List<Claim> GetClaims(User user, string scope)
        {
            var claims = new List<Claim>();
            if (scope == "email")
            {
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    claims.Add(new Claim("email", user.Email));
                }
            }

            if (scope == "role")
            {
                if (!string.IsNullOrWhiteSpace(user.Role))
                {
                    claims.Add(new Claim("role", user.Role));
                }
            }

            return claims;
        }

        #endregion Private Methods
    }
}