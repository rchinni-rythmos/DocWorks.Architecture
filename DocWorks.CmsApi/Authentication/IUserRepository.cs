namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for User Repository
    /// </summary>
    public interface IUserRepository
    {
        // Validate user credentials
        Task<bool> ValidateCredentialsAsync(string userPrincipalName, string password);

        // Find user information by passing userPrincipalName
        Task<ApplicationUser> FindByUserPrincipalNameAsync(string userPrincipalName);
    }
}
