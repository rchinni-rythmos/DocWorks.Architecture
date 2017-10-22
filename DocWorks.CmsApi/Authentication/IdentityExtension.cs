namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DocWorks.CMS.Api.Authentication;

    /// <summary>
    /// Custom identity server extension for LDAP Authentication
    /// </summary>
    public static class CustomIdentityServerBuilderExtensions
    {
        /// <summary>
        /// Add user custom store
        /// </summary>
        /// <param name="builder">This is IIdentityServerBuilder reference</param>
        /// <returns>IIdentityServerBuilder reference</returns>
        public static IIdentityServerBuilder AddCustomUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}
