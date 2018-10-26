namespace AspNetCore.Security.Jwt
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName("AspNetCore.Security.Jwt")));

            return mvcBuilder;
        }

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
