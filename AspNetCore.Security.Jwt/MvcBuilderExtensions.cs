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

        private static bool IsUserModelSecurityAdded { get; set; }
        private static bool IsFacebookSecurityAdded { get; set; }

        /// <summary>
        /// Add Security extension. Adds Security assembly for default Authentication.
        /// </summary>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName(ASSEMBLY_NAME)))
                      .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new RemoveControllerFeatureProvider()))
                      .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new TokenControllerFeatureProvider()))
                      .ConfigureApplicationPartManager(IsFacebookSecurityAdded, apm =>
                                   apm.FeatureProviders.Add(new FacebookControllerFeatureProvider() { AddFacebookController = true }));

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
            if (!IsUserModelSecurityAdded)
            {
                mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName(ASSEMBLY_NAME)))
                          .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new RemoveControllerFeatureProvider()))
                          .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new GenericTokenControllerFeatureProvider<TUserModel>()))
                          .ConfigureApplicationPartManager(IsFacebookSecurityAdded, apm =>
                                   apm.FeatureProviders.Add(new FacebookControllerFeatureProvider() { AddFacebookController = true }));
            }
            
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
            if (!IsFacebookSecurityAdded)
            {
                mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName(ASSEMBLY_NAME)))
                          .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new RemoveControllerFeatureProvider()))
                          .ConfigureApplicationPartManager(apm =>
                                   apm.FeatureProviders.Add(new FacebookControllerFeatureProvider() { AddFacebookController = true }));
            }

            IsFacebookSecurityAdded = true;

            return mvcBuilder;
        }        

        public static IMvcBuilder ConfigureApplicationPartManager(this IMvcBuilder builder, bool addIf, Action<ApplicationPartManager> setupAction)
        {
            if (addIf)
            {
                builder.ConfigureApplicationPartManager(setupAction);
            }

            return builder;
        }
    }
}
