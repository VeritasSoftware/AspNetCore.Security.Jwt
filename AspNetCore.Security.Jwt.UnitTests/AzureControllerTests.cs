using AspNetCore.Security.Jwt.AzureAD;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class AzureControllerTests
    {
        private Mock<AzureClient> MockAzureClient { get; set; }
        private AzureADSecuritySettings SecuritySettings { get; set; }

        internal Mock<AzureClient> InitMockAzureClient(AzureADSecuritySettings securitySettings, bool isAuthenticated = true)
        {            
            var securityClient = new Mock<AzureClient>(securitySettings);
            securityClient.Setup(x => x.PostSecurityRequest()).ReturnsAsync(() => new AzureADResponseModel
            {
                IsAuthenticated = isAuthenticated,
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSm9obiBEb2UiLCJleHAiOjE1NDM2MzAzNzcsIm5iZiI6MTU0MzYyNjA1NywiaXNzIjoieW91ciBhcHAiLCJhdWQiOiJ0aGUgY2xpZW50IG9mIHlvdXIgYXBwIn0.8dv4RankRw7n4OoTmqWMNFWnaTjXzobz_mUkfTh13zI"
            });

            return securityClient;
        }

        public AzureControllerTests()
        {
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
                    APIKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
                }
            };

            this.SecuritySettings = securitySettings.AzureADSecuritySettings;

            this.MockAzureClient = this.InitMockAzureClient(this.SecuritySettings);
        }

        [Fact]
        public async Task Test_AzureController_Pass()
        {
            //Arrange
            AzureADAuthModel azureADAuthModel = new AzureADAuthModel
            {
                APIKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            AzureAuthenticator azureAuthenticator = new AzureAuthenticator(this.SecuritySettings, this.MockAzureClient.Object);

            var controller = new AzureController(azureAuthenticator);

            //Act
            var result = await controller.Create(azureADAuthModel);

            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True((result as ObjectResult).Value.ToString().IsValidJwtToken());
            this.MockAzureClient.Verify(x => x.PostSecurityRequest(), Times.Once);
        }

        [Fact]
        public async Task Test_AzureController_AzureAuth_Fail()
        {
            //Arrange

            //Azure Client returns IsAuthenticated false
            this.MockAzureClient = this.InitMockAzureClient(this.SecuritySettings, false);

            AzureADAuthModel azureADAuthModel = new AzureADAuthModel
            {
                APIKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            AzureAuthenticator azureAuthenticator = new AzureAuthenticator(this.SecuritySettings, this.MockAzureClient.Object);

            var controller = new AzureController(azureAuthenticator);

            //Act
            var result = await controller.Create(azureADAuthModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            this.MockAzureClient.Verify(x => x.PostSecurityRequest(), Times.Once);
        }

        [Fact]
        public async Task Test_AzureController_InvalidAPIKey_Fail()
        {
            //Arrange

            //Invalid API Key
            AzureADAuthModel azureADAuthModel = new AzureADAuthModel
            {
                APIKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            AzureAuthenticator azureAuthenticator = new AzureAuthenticator(this.SecuritySettings, this.MockAzureClient.Object);

            var controller = new AzureController(azureAuthenticator);

            //Act
            var result = await controller.Create(azureADAuthModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            this.MockAzureClient.Verify(x => x.PostSecurityRequest(), Times.Never);
        }
    }
}
