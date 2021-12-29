using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class SwaggerExtensionsTests
    {
        IConfiguration Configuration { get; set; }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("securitySettings.json")
                .Build();
            return config;
        }

        public SwaggerExtensionsTests()
        {
            this.Configuration = InitConfiguration();
        }

        [Fact]
        public void Test_SwaggerExtensions_AddSecureSwaggerDocumentation_Pass()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecureSwaggerDocumentation();

            Assert.True(SwaggerExtensions.IsSwaggerAdded);
        }

        [Fact]
        public void Test_SwaggerExtensions_UseSwaggerDocumentation_Pass()
        {
            var serviceCollection = new ServiceCollection();
            var mockEnv = new Mock<IWebHostEnvironment>();
            serviceCollection.AddScoped(x => mockEnv.Object);
            serviceCollection.AddSecurity<DefaultAuthenticator>(this.Configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var app = new ApplicationBuilder(serviceProvider);

            app.UseSwaggerDocumentation();

            Assert.True(SwaggerExtensions.IsSwaggerSecurityUsed);
        }
    }
}
