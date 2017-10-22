namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class for Custom profile of user, to get data from active directory
    /// </summary>
    public class CustomProfileService : IProfileService
    {
        protected readonly ILogger _logger;
        protected readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProfileService"/> class.
        /// </summary>
        /// <param name="userRepository"> This is IUserRepository reference </param>
        /// <param name="logger"> This is ILogger reference </param>
        public CustomProfileService(IUserRepository userRepository, ILogger<CustomProfileService> logger)
        {
            this._userRepository = userRepository;
            this._logger = logger;
        }

        /// <summary>
        /// Get profiel data of logged in user from active directory
        /// </summary>
        /// <param name="context">Object of ProfileDataRequestContext class</param>
        /// <returns>Task empty object</returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string userPrincipalName = context.Subject.GetSubjectId();
            ApplicationUser objApplicationUser = await this._userRepository.FindByUserPrincipalNameAsync(userPrincipalName);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, objApplicationUser.ObjectSid),
                new Claim(ClaimTypes.GivenName, objApplicationUser.FirstName),
                new Claim(ClaimTypes.Surname, objApplicationUser.LastName),
                new Claim(ClaimTypes.Upn, objApplicationUser.UserPrincipalName),
            };

            context.IssuedClaims = claims;
        }

        /// <summary>
        /// Check whether is still active in active directory or not
        /// </summary>
        /// <param name="context">Object of IsActiveContext class</param>
        /// <returns>Task empty object</returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            string userPrincipalName = context.Subject.GetSubjectId();
            ApplicationUser objApplicationUser = await this._userRepository.FindByUserPrincipalNameAsync(userPrincipalName);
            context.IsActive = objApplicationUser != null;
        }
    }
}
