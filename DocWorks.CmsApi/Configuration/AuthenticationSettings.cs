// <copyright file="LDAPSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DocWorks.CMS.Api.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// This class is for custom app settings related to LDAP Authentication
    /// </summary>
    public class AuthenticationSettings
    {
        // google site url for verifying captcha
        public static string CaptchaVerifySiteUrl { get; set; }

        // Secret key for captcha
        public static string CaptchaSecret { get; set; }

        // Ignore captcha validation, helpful in automation and testing
        public static bool IgnoreCaptchaValidation { get; set; }

        // URL of the hosted application
        public static string WebAppURL { get; set; }

        // Public IP Address of LDAP server
        public static string LDAPHostServerPublicIP { get; set; }

        // Search base of active directory root (it can be organizational unit also)
        public static string LDAPHostServerSearchBase { get; set; }

        // UPN(UserPrincipalName) of Active directory user for establishing active directory connection
        public static string LDAPLoginUPN { get; set; }

        // Passowrd of Active directory user for establishing active directory connection
        public static string LDAPLoginPassword { get; set; }

        // Lifetime of access token in seconds
        public static string AccessTokenLifetime { get; set; }
    }
}
