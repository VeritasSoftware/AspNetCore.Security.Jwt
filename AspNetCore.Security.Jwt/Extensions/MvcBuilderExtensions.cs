namespace AspNetCore.Security.Jwt
{
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Reflection;

    /// <summary>
    /// MvcBuilder extensions
    /// </summary>
    public static class MvcBuilderExtensions
    {
        const string ASSEMBLY_NAME = "AspNetCore.Security.Jwt";

        internal static bool IsDefaultSecurityAdded { get; set; }
        internal static bool IsUserModelSecurityAdded { get; set; }
        internal static bool IsFacebookSecurityAdded { get; set; }
        internal static bool IsAzureSecurityAdded { get; set; }

        /// <summary>
        /// Add Security extension. Adds Security assembly for default Authentication.
        /// </summary>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.SecurityInit()
                      .ConfigureApplicationPartManager(!IsDefaultSecurityAdded && !IsUserModelSecurityAdded, 
                                                            apm => apm.FeatureProviders.Add(new TokenControllerFeatureProvider()));                 

            IsDefaultSecurityAdded = true;

            return mvcBuilder;
        }

        /// <summary>
        /// Add Security extension. Adds Security assembly and support for custom User model Authentication.
        /// </summary>
        /// <typeparam name="TUserModel">The custom User model</typeparam>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddSecurity<TUserModel>(this IMvcBuilder mvcBuilder)
            where TUserModel: class, IAuthenticationUser            
        {
            mvcBuilder.SecurityInit()
                      .ConfigureApplicationPartManager(!IsDefaultSecurityAdded && !IsUserModelSecurityAdded, 
                                                            apm => apm.FeatureProviders.Add(new GenericTokenControllerFeatureProvider<TUserModel>()));            
            
            IsUserModelSecurityAdded = true;            

            return mvcBuilder;
        }

        /// <summary>
        /// Add Facebook security extension
        /// </summary>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddFacebookSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.SecurityInit()
                      .ConfigureApplicationPartManager(!IsFacebookSecurityAdded, 
                                                            apm => apm.FeatureProviders.Add(new FacebookControllerFeatureProvider() { AddFacebookController = true }));            

            IsFacebookSecurityAdded = true;

            return mvcBuilder;
        }

        /// <summary>
        /// Add Azure AD security extension
        /// </summary>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddAzureADSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.SecurityInit()
                      .ConfigureApplicationPartManager(!IsAzureSecurityAdded,
                                                            apm => apm.FeatureProviders.Add(new AzureControllerFeatureProvider() { AddAzureController = true }));

            IsAzureSecurityAdded = true;

            return mvcBuilder;
        }

        internal static IMvcBuilder ConfigureApplicationPartManager(this IMvcBuilder builder, bool addIf, Action<ApplicationPartManager> setupAction)
        {
            if (addIf)
            {
                builder.ConfigureApplicationPartManager(setupAction);
            }

            return builder;
        }

        internal static IMvcBuilder SecurityInit(this IMvcBuilder mvcBuilder)
        {
            if (!IsDefaultSecurityAdded && !IsUserModelSecurityAdded && !IsFacebookSecurityAdded)
            {
                mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName(ASSEMBLY_NAME)))
                          .ConfigureApplicationPartManager(apm =>
                                         apm.FeatureProviders.Add(new RemoveControllerFeatureProvider()));
            }

            return mvcBuilder;
        }
    }
}
