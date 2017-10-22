namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DocWorks.CMS.Api.Authentication;
    using DocWorks.CMS.Api.Configuration;
    using Novell.Directory.Ldap;
    using Novell.Directory.Ldap.Utilclass;

    /// <summary>
    /// Class for handling all Active Directory related tasks
    /// </summary>
    public static class ActiveDirectoryHelper
    {
        /// <summary>
        /// Connect with LDAP server
        /// </summary>
        /// <returns>LDAP Connection object</returns>
        public static LdapConnection GetLDAPConnnection()
        {
            int ldapPort = LdapConnection.DEFAULT_PORT;
            string ldapHost = AuthenticationSettings.LDAPHostServerPublicIP;
            LdapConnection conn = new LdapConnection();

            // trying to connect to the LDAP server
            // if connected then return true otherwise exception will be bubbled to common exception handler
            conn.Connect(ldapHost, ldapPort);
            return conn;
        }

        /// <summary>
        /// Check whether user is valid in active directory or not
        /// </summary>
        /// <param name="userPrincipalName">UserPrincipalName of user</param>
        /// <param name="password">Passowrd of user</param>
        /// <returns>Bool result, indicate whether user credentials are valid or not</returns>
        public static async Task<bool> CheckUserValidInActiveDirectoryAsync(string userPrincipalName, string password)
        {
            using (LdapConnection conn = ActiveDirectoryHelper.GetLDAPConnnection())
            {
                try
                {
                    conn.Bind(userPrincipalName, password);
                    await Task.Yield();
                    return true;
                }
                catch (LdapException ex)
                {
                    // TODO: log exception
                    return false;
                }
            }
        }

        /// <summary>
        /// Find user in Active Directory by using UPN
        /// </summary>
        /// <param name="userPrincipalName">UserPrincipalName of user</param>
        /// <returns>Return CustomUser object</returns>
        public static async Task<ApplicationUser> FindByUserPrincipalNameAsync(string userPrincipalName)
        {
            string loginDN = AuthenticationSettings.LDAPLoginUPN;
            string password = AuthenticationSettings.LDAPLoginPassword;
            string searchBase = AuthenticationSettings.LDAPHostServerSearchBase;
            string searchFilter = "(userPrincipalName=" + userPrincipalName + ")";

            ApplicationUser objApplicationUser = new ApplicationUser();
            LdapSearchResults lsc = null;

            using (LdapConnection conn = ActiveDirectoryHelper.GetLDAPConnnection())
            {
                conn.Bind(loginDN, password);
                lsc = conn.Search(searchBase, LdapConnection.SCOPE_SUB, searchFilter, null, false);

                // Only one record yield if search through UPN in active directory
                while (lsc.hasMore())
                {
                    LdapEntry nextEntry = null;

                    nextEntry = lsc.next();

                    LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
                    System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                    Dictionary<string, string> adProperties = new Dictionary<string, string>();

                    while (ienum.MoveNext())
                    {
                        LdapAttribute attribute = (LdapAttribute)ienum.Current;
                        string attributeName = attribute.Name;
                        string attributeVal = attribute.StringValue;

                        if (!Base64.isLDIFSafe(attributeVal))
                        {
                            byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                            attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                        }

                        adProperties.Add(attributeName, attributeVal);
                    }

                    string objectSid = string.Empty;
                    string givenName = string.Empty;
                    string sn = string.Empty;
                    string upn = string.Empty;

                    adProperties.TryGetValue("objectSid", out objectSid);
                    adProperties.TryGetValue("givenName", out givenName);
                    adProperties.TryGetValue("sn", out sn);
                    adProperties.TryGetValue("userPrincipalName", out upn);

                    objApplicationUser.ObjectSid = objectSid;
                    objApplicationUser.FirstName = givenName;
                    objApplicationUser.LastName = sn;
                    objApplicationUser.UserPrincipalName = upn;

                    await Task.Yield();

                    return objApplicationUser;
                }
            }

            return null;
        }
    }
}
