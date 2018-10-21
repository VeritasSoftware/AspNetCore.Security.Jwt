namespace Api.Security.Jwt
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddSecurity(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName("Api.Security.Jwt")));

            return mvcBuilder;
        }
    }
}
