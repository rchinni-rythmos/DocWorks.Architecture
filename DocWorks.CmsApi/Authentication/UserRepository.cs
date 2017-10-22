namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DocWorks.CMS.Api.Authentication;

    /// <summary>
    /// Class for user repository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <param name="userPrincipalName">User principal name of user for login</param>
        /// <param name="password">Password of user for login</param>
        /// <returns>Bool result, indicate whether user credentials are valid or not</returns>
        public async Task<bool> ValidateCredentialsAsync(string userPrincipalName, string password)
        {
            return await ActiveDirectoryHelper.CheckUserValidInActiveDirectoryAsync(userPrincipalName, password);
        }

        /// <summary>
        /// Find user information by passing userPrincipalName
        /// </summary>
        /// <param name="userPrincipalName">User principal name of user for login</param>
        /// <returns>CustomUser class object</returns>
        public async Task<ApplicationUser> FindByUserPrincipalNameAsync(string userPrincipalName)
        {
            return await ActiveDirectoryHelper.FindByUserPrincipalNameAsync(userPrincipalName);
        }
    }
}
