namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DocWorks.CMS.Api.Configuration;
    using IdentityServer4;
    using IdentityServer4.Models;
    using System.Security.Claims;

    /// <summary>
    /// Class for configuration of client and api resources for IdentityServer4
    /// </summary>
    public static class AuthenticationConfig
    {
        /// <summary>
        /// Scopes define the API resources in your system
        /// </summary>
        /// <returns>List of Api Resources</returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("CMSApi", "CMSApi"),
            };
        }

        /// <summary>
        /// Client want to access resources (aka scopes)
        /// </summary>
        /// <returns>List of Client</returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // resource owner password grant client
                new Client
                {
                    ClientId = "CMSApiClient",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        "CMSApi",
                    },
                    AccessTokenLifetime = Convert.ToInt32(AuthenticationSettings.AccessTokenLifetime),
                },
            };
        }
    }
}
