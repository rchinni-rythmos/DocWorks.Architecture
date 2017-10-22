namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for application user for getting data from LDAP server
    /// </summary>
    public class ApplicationUser
    {
        // Unique identifier for every object in active directory
        public string ObjectSid { get; set; }

        // Given name of user
        public string FirstName { get; set; }

        // Surname of user
        public string LastName { get; set; }

        // UserPrincipalName of user (sometimes referred to as email also but can be different from email depends on email server settings)
        public string UserPrincipalName { get; set; }
    }
}
