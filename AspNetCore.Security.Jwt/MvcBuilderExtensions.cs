namespace AspNetCore.Security.Jwt
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    /// <summary>
    /// MvcBuilder extensions
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Add Security extension. Adds Security assembly for default Authentication.
        /// </summary>
        /// <param name="mvcBuilder">The MvcBuilder</param>
        /// <returns><see cref="IMvcBuilder"/></returns>
        public static IMvcBuilder AddSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName("AspNetCore.Security.Jwt")));

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
            mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName("AspNetCore.Security.Jwt")))
                      .ConfigureApplicationPartManager(apm =>
                                    apm.FeatureProviders.Add(new GenericControllerFeatureProvider<TUserModel>()));

            return mvcBuilder;
        }
    }
}
