using AspNetCore.Security.Jwt.AzureAD;
using AspNetCore.Security.Jwt.Facebook;
using AspNetCore.Security.Jwt.Google;
using AspNetCore.Security.Jwt.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    /// <summary>
    /// Tests to verify that the Security Extensions are registering the DI properly.
    /// </summary>
    public class SecurityExtensionsTests
    {
        IConfiguration Configuration { get; set; }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("securitySettings.json")
                .Build();
            return config;
        }

        public SecurityExtensionsTests()
        {
            this.Configuration = InitConfiguration();
        }

        [Fact]
        public void Test_SecurityExtensions_Default_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity<DefaultAuthenticator>(this.Configuration, true);

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
        public void Test_SecurityExtensions_CustomUserModel_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSecurity<CustomAuthenticator, UserModel>(this.Configuration, builder => builder.AddClaim("ABC", "XYZ"));

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authService = sp.GetService<IAuthentication<UserModel>>();
            var securityService = sp.GetService<ISecurityService<UserModel>>();

            //Assert
            Assert.True(authService != null);
            Assert.IsType<CustomAuthenticator>(authService);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService<UserModel>>(securityService);
        }

        [Fact]
        public void Test_SecurityExtensions_AzureAD_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAzureADSecurity(this.Configuration, true);

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var azureADSecuritySettings = sp.GetService<AzureADSecuritySettings>();
            var authAzure = sp.GetService<IAuthentication<AzureADAuthModel, AzureADResponseModel>>();
            var securityClient = sp.GetService<ISecurityClient<AzureADResponseModel>>();

            //Assert            
            Assert.True(azureADSecuritySettings != null);
            Assert.IsType<AzureADSecuritySettings>(azureADSecuritySettings);

            var securitySettings = new SecuritySettings();
            this.Configuration.Bind("SecuritySettings", securitySettings);

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
        }

        [Fact]
        public void Test_SecurityExtensions_Facebook_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddFacebookSecurity(this.Configuration, builder => 
                                                    builder.AddClaim("FacebookUser", userModel => userModel.UserAccessToken));

            var sp = serviceCollection.BuildServiceProvider();

            //Act
            var authFacebook = sp.GetService<IAuthentication<FacebookAuthModel>>();
            var securityService = sp.GetService<ISecurityService<FacebookAuthModel>>();
            var securityClient = sp.GetService<ISecurityClient<FacebookAuthModel, bool>>();

            //Assert            
            Assert.True(authFacebook != null);
            Assert.IsType<FacebookAuthenticator>(authFacebook);

            Assert.True(securityService != null);
            Assert.IsType<SecurityService<FacebookAuthModel>>(securityService);

            Assert.True(securityClient != null);
            Assert.IsType<FacebookClient>(securityClient);
        }

        [Fact]
        public void Test_SecurityExtensions_Google_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddGoogleSecurity(this.Configuration, true);
            
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
        public void Test_SecurityExtensions_Twitter_Pass()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTwitterSecurity(this.Configuration, true);

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
        }

        [Fact]
        public void Test_SecurityExtensions_UseSecurity_Pass()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSecurity<DefaultAuthenticator>(this.Configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var app = new ApplicationBuilder(serviceProvider);

            app.UseSecurity();

            Assert.True(SecurityExtensions.IsSecurityUsed);
        }

    }
}
