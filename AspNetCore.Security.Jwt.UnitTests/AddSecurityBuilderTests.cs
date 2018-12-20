using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using AspNetCore.Security.Jwt.Google;
using AspNetCore.Security.Jwt.Twitter;
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

            serviceCollection.AddScoped<TokenController>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authService = sp.GetService<IAuthentication>();
            var securityService = sp.GetService<ISecurityService>();
            var controller = sp.GetService<TokenController>();

            //Assert
            Assert.True(authService != null);
            Assert.IsType<DefaultAuthenticator>(authService);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService>(securityService);

            Assert.True(controller != null);
        }

        [Fact]
        public void Test_AddSecurityBuilder_DefaultTwice_Pass()
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
                             .AddSecurity<DefaultAuthenticator>()
                             .AddSecurity<DefaultAuthenticator>();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddSecurity<DefaultAuthenticator>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authService = sp.GetService<IAuthentication>();
            var securityService = sp.GetService<ISecurityService>();

            //Assert
            Assert.True(authService != null);
            Assert.IsType<DefaultAuthenticator>(authService);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService>(securityService);
        }

        [Fact]
        public void Test_AddSecurityBuilder_CustomUserModel_Pass()
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
                             .AddSecurity<CustomAuthenticator, UserModel>();

            serviceCollection.AddScoped<TokenController<UserModel>>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authService = sp.GetService<IAuthentication<UserModel>>();
            var securityService = sp.GetService<ISecurityService<UserModel>>();
            var controller = sp.GetService<TokenController<UserModel>>();

            //Assert
            Assert.True(authService != null);
            Assert.IsType<CustomAuthenticator>(authService);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService<UserModel>>(securityService);

            Assert.True(controller != null);
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

            serviceCollection.AddScoped<AzureController>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var azureADSecuritySettings = sp.GetService<AzureADSecuritySettings>();
            var authAzure = sp.GetService<IAuthentication<AzureADAuthModel, AzureADResponseModel>>();
            var securityClient = sp.GetService<ISecurityClient<AzureADResponseModel>>();
            var controller = sp.GetService<AzureController>();

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

            Assert.True(securityClient != null);
            Assert.IsType<AzureClient>(securityClient);
            Assert.True(controller != null);
        }

        [Fact]
        public void Test_AddSecurityBuilder_Facebook_Pass()
        {
            //Arrange
            SecuritySettings securitySettings = new SecuritySettings()
            {
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AppId = "xxxxxxxxxxxxxx",
                AppSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddFacebookSecurity();

            serviceCollection.AddScoped<FacebookController>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var securityClient = sp.GetService<ISecurityClient<FacebookAuthModel, bool>>();
            var authFacebook = sp.GetService<IAuthentication<FacebookAuthModel>>();
            var securityService = sp.GetService<ISecurityService<FacebookAuthModel>>();
            var controller = sp.GetService<FacebookController>();

            //Assert            
            Assert.True(authFacebook != null);
            Assert.IsType<FacebookAuthenticator>(authFacebook);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService<FacebookAuthModel>>(securityService);

            Assert.True(securityClient != null);
            Assert.IsType<FacebookClient>(securityClient);

            Assert.True(controller != null);
        }

        [Fact]
        public void Test_AddSecurityBuilder_Google_Pass()
        {
            //Arrange
            SecuritySettings securitySettings = new SecuritySettings()
            {
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AppId = "xxxxxxxxxxxxxx",
                AppSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                GoogleSecuritySettings = new GoogleSecuritySettings
                {
                    APIKey = "<api key>",
                    ClientId = "<client id>",
                    ClientSecret = "<client secret>",
                    RedirectUri = "http://localhost/"
                }
            };

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddGoogleSecurity();

            serviceCollection.AddScoped<GoogleController>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var securityClient = sp.GetService<ISecurityClient<GoogleAuthModel, GoogleResponseModel>>();
            var autheticator = sp.GetService<IAuthentication<GoogleAuthModel, GoogleResponseModel>>();            
            var controller = sp.GetService<GoogleController>();

            //Assert            
            Assert.True(securityClient != null);
            Assert.IsType<GoogleClient>(securityClient);

            Assert.True(autheticator != null);
            Assert.IsType<GoogleAuthenticator>(autheticator);                       

            Assert.True(controller != null);
        }

        [Fact]
        public void Test_AddSecurityBuilder_Twitter_Pass()
        {
            //Arrange
            SecuritySettings securitySettings = new SecuritySettings()
            {
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AppId = "xxxxxxxxxxxxxx",
                AppSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                TwitterSecuritySettings = new TwitterSecuritySettings
                {
                    APIKey = "<api key>",
                    ConsumerKey = "<consumer key>",
                    ConsumerSecret = "<consumer secret>"
                }
            };

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity(securitySettings, false)
                             .AddTwitterSecurity();

            serviceCollection.AddScoped<TwitterController>();

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var securityClient = sp.GetService<ISecurityClient<TwitterResponseModel>>();
            var autheticator = sp.GetService<IAuthentication<TwitterAuthModel, TwitterResponseModel>>();
            var controller = sp.GetService<TwitterController>();

            //Assert            
            Assert.True(securityClient != null);
            Assert.IsType<TwitterClient>(securityClient);

            Assert.True(autheticator != null);
            Assert.IsType<TwitterAuthenticator>(autheticator);

            Assert.True(controller != null);
            Assert.IsType<TwitterController>(controller);
        }

    }
}
