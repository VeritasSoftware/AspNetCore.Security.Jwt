using AspNetCore.Security.Jwt.AzureAD;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    /// <summary>
    /// Tests to verify that the Builder APIs are registering the DI properly.
    /// </summary>
    public class AddSecurityBuilderTests
    {
        [Fact]
        public void Test_AddSecurityBuilder_Default_Pass()
        {
            //Arrange
            SecuritySettings securitySettings = new SecuritySettings()
            {
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
            };

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddSecurity<DefaultAuthenticator>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authService = sp.GetService<IAuthentication>();
            var securityService = sp.GetService<ISecurityService>();

            Assert.True(authService != null);
            Assert.IsType<DefaultAuthenticator>(authService);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService>(securityService);
        }

        [Fact]
        public void Test_AddSecurityBuilder_AzureAD_Pass()
        {
            //Arrange
            SecuritySettings securitySettings = new SecuritySettings()
            {
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AzureADSecuritySettings = new AzureADSecuritySettings
                {
                    AADInstance = "https://login.windows.net/{0}",
                    Tenant = "<B2BADTenant>.onmicrosoft.com",
                    ResourceId = "https://<B2BADTenant>.onmicrosoft.com/<azureappname>",
                    ClientId = "<client-id-web-add>",
                    ClientSecret = "<client-secret>",
                    APIKey = Guid.NewGuid().ToString()
                }
            };

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddAzureADSecurity();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var azureADSecuritySettings = sp.GetService<AzureADSecuritySettings>();
            var authAzure = sp.GetService<IAuthentication<AzureADAuthModel, AzureADResponseModel>>();

            //Assert            
            Assert.True(azureADSecuritySettings != null);
            Assert.IsType<AzureADSecuritySettings>(azureADSecuritySettings);

            Assert.True(azureADSecuritySettings.AADInstance == securitySettings.AzureADSecuritySettings.AADInstance);
            Assert.True(azureADSecuritySettings.Tenant == securitySettings.AzureADSecuritySettings.Tenant);
            Assert.True(azureADSecuritySettings.ResourceId == securitySettings.AzureADSecuritySettings.ResourceId);
            Assert.True(azureADSecuritySettings.ClientId == securitySettings.AzureADSecuritySettings.ClientId);
            Assert.True(azureADSecuritySettings.ClientSecret == securitySettings.AzureADSecuritySettings.ClientSecret);
            Assert.True(azureADSecuritySettings.APIKey == securitySettings.AzureADSecuritySettings.APIKey);

            Assert.True(authAzure != null);
            Assert.IsType<AzureAuthenticator>(authAzure);
        }

    }
}
