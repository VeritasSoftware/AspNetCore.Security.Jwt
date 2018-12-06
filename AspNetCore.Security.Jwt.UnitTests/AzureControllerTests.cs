using AspNetCore.Security.Jwt.AzureAD;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
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
                AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjFMVE16YWtpaGlSbGFfOHoyQkVKVlhlV01xbyJ9.eyJ2ZXIiOiIyLjAiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vOTE4ODA0MGQtNmM2Ny00YzViLWIxMTItMzZhMzA0YjY2ZGFkL3YyLjAiLCJzdWIiOiJBQUFBQUFBQUFBQUFBQUFBQUFBQUFJa3pxRlZyU2FTYUZIeTc4MmJidGFRIiwiYXVkIjoiNmNiMDQwMTgtYTNmNS00NmE3LWI5OTUtOTQwYzc4ZjVhZWYzIiwiZXhwIjoxNTM2MzYxNDExLCJpYXQiOjE1MzYyNzQ3MTEsIm5iZiI6MTUzNjI3NDcxMSwibmFtZSI6IkFiZSBMaW5jb2xuIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiQWJlTGlAbWljcm9zb2Z0LmNvbSIsIm9pZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC02NmYzLTMzMzJlY2E3ZWE4MSIsInRpZCI6IjMzMzgwNDBkLTZjNjctNGM1Yi1iMTEyLTM2YTMwNGI2NmRhZCIsIm5vbmNlIjoiMTIzNTIzIiwiYWlvIjoiRGYyVVZYTDFpeCFsTUNXTVNPSkJjRmF0emNHZnZGR2hqS3Y4cTVnMHg3MzJkUjVNQjVCaXN2R1FPN1lXQnlqZDhpUURMcSFlR2JJRGFreXA1bW5PcmNkcUhlWVNubHRlcFFtUnA2QUlaOGpZIn0=.1AFWW-Ck5nROwSlltm7GzZvDwUkqvhSQpm55TQsmVo9Y59cLhRXpvB8n-55HCr9Z6G_31_UbeUkoz612I2j_Sm9FFShSDDjoaLQr54CreGIJvjtmS3EkK9a7SJBbcpL1MpUtlfygow39tFjY7EVNW9plWUvRrTgVk7lYLprvfzw-CIqw3gHC-T7IK_m_xkr08INERBtaecwhTeN4chPC4W3jdmw_lIxzC48YoQ0dB1L9-ImX98Egypfrlbm0IBL5spFzL6JDZIRRJOu8vecJvj1mq-IUhGt0MacxX8jdxYLP-KUu2d9MbNKpCKJuZ7p8gwTL5B7NlUdh_dmSviPWrw"
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

        [Fact]
        public async Task Test_AzureController_SecurityException_Fail()
        {
            //Arrange

            //Authorization Code absent
            AzureADAuthModel googleAuthModel = new AzureADAuthModel
            {
                APIKey = "<api key>"
            };

            AzureAuthenticator authenticator = new AzureAuthenticator(this.SecuritySettings,
                                                                        this.MockAzureClient.Object);

            var controller = new AzureController(authenticator);

            try
            {
                //Act
                var result = await controller.Create(googleAuthModel);
            }
            catch (SecurityException ex)
            {
                //Assert
                Assert.IsType<SecurityException>(ex);
                this.MockAzureClient.Verify(x => x.PostSecurityRequest(), Times.Never);
            }
        }

        [Fact]
        public async Task Test_AzureController_AzureAuthorizeAttribute_InvalidAPIKey_ReturnsUnauthorizedResult()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("securitySettings.json")
                .Build();

            // Arrange
            var server = new TestServer(new WebHostBuilder()
                                .UseConfiguration(config)
                                .UseStartup<Startup>());
            var client = server.CreateClient();
            var url = "/azure";
            var expected = HttpStatusCode.Unauthorized;

            AzureADAuthModel azureADAuthModel = new AzureADAuthModel
            {
                APIKey = "invalid api key"
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(azureADAuthModel));

            // Act
            var response = await client.PostAsync(url, httpContent);

            // Assert
            Assert.Equal(expected, response.StatusCode);

            //Arrange
            var bytes = new byte[2] { 103, 104 };

            httpContent = new ByteArrayContent(bytes);
            
            try
            {
                // Act
                response = await client.PostAsync(url, httpContent);
            }
            catch (SecurityException ex)
            {
                // Assert
                Assert.IsType<SecurityException>(ex);
            }
        }

    }
}