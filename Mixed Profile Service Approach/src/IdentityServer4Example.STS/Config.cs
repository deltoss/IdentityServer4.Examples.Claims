// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4Example.STS
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { JwtClaimTypes.Role }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("weatherforecastapi.read", "Read access to the Weather Forecast API"),
                new ApiScope("weatherforecastapi.write", "Write access to the Weather Forecast API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource
                {
                    Name = "weatherforecastapi",
                    DisplayName = "Weather Forecast Api",
                    Description = "Allow the application to access the Weather Forecast API on your behalf.",
                    Scopes = new List<string> { "weatherforecastapi.read", "weatherforecastapi.write" },
                    // Defines what additional claims (i.e. through specifying the Identity Resource names)
                    // gets included in the access token or id token that gets retrieved from the STS.
                    UserClaims = new List<string> { "role" },
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes =
                    {
                        "weatherforecastapi.read",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email
                    },
                }
            };
    }
}