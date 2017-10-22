namespace DocWorks.CMS.Api.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4.Validation;
    using DocWorks.CMS.Api.Configuration;
    using IdentityServer4.Models;
    using System.Security.Claims;

    /// <summary>
    /// Class for validating the user from active directory
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomResourceOwnerPasswordValidator"/> class
        /// </summary>
        /// <param name="userRepository">IUserRepository reference</param>
        public CustomResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        /// <summary>
        /// Validate user's credentials are valid or not in LDAP server
        /// </summary>
        /// <param name="context"> This is the context of user from which we can get provided username and password data</param>
        /// <returns>Task object</returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            bool captchaResult = true;

            // If IgnoreCaptchaValidation config is false then only run captcha validation logic
            if (!AuthenticationSettings.IgnoreCaptchaValidation)
            {
                // get the captcha response token from client
                string captchaResponseToken = context.Request.Raw["captchaResponseToken"];

                // validate the captcha response token coming from client
                captchaResult = await CaptchaVerification.ValidateCaptchaResponse(captchaResponseToken);
            }

            // if the captcha validation response is valid then only check for credentials
            // otherwise return from else condition
            if (captchaResult)
            {
                if (await this._userRepository.ValidateCredentialsAsync(context.UserName, context.Password))
                {
                    ApplicationUser objApplicationUser = await this._userRepository.FindByUserPrincipalNameAsync(context.UserName);

                    Dictionary<string, object> customResponse = new Dictionary<string, object>();
                    customResponse.Add("firstName", objApplicationUser.FirstName);
                    customResponse.Add("lastName", objApplicationUser.LastName);
                    customResponse.Add("userPrincipalName", objApplicationUser.UserPrincipalName);

                    context.Result = new GrantValidationResult(context.UserName, OidcConstants.AuthenticationMethods.Password, null, "local", customResponse);
                }
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Captcha Response Token");
            }
        }
    }
}
