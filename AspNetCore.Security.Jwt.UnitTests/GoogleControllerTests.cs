using AspNetCore.Security.Jwt.Google;
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
    public class GoogleControllerTests
    {
        private Mock<GoogleClient> MockGoogleClient { get; set; }
        private Mock<HttpClientHandler> MockHttpClient { get; set; }
        private SecuritySettings SecuritySettings { get; set; }

        internal Mock<GoogleClient> InitMockGoogleClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var securityClient = new Mock<GoogleClient>(securitySettings, It.IsAny<HttpClientHandler>());
            securityClient.Setup(x => x.PostSecurityRequest(It.IsAny<GoogleAuthModel>())).ReturnsAsync(() => new GoogleResponseModel
            {
                IsAuthenticated = isAuthenticated,
                AccessToken = "ya29.GltoBsrJ_RRZzI-DEzU3l6nDz_qwy7RFM-zFv7MA1z6ZeU3IijEZa_ECHG70V-cFz7omdplXraYVjTvrZkkYqdaf0Z8-vnQ6NiLeOXW3GLCqnlYjabwf59RMaUv8",
                RefreshToken = "1/PN_s59ZaV_pPDvYjM-EDeUlxg9OHTh8gzdAGmgsERrM",
                ExpiresIn = 3600,
                Scope = "create",
                TokenType = "bearer"
            });

            return securityClient;
        }

        internal Mock<HttpClientHandler> InitMockHttpClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var httpClient = new Mock<HttpClientHandler>();
            httpClient.Setup(x => x.SendAsync<GoogleResponseModel>(It.IsAny<HttpRequestMessage>())).ReturnsAsync(() => new GoogleResponseModel
            {
                IsAuthenticated = isAuthenticated,
                AccessToken = "ya29.GltoBsrJ_RRZzI-DEzU3l6nDz_qwy7RFM-zFv7MA1z6ZeU3IijEZa_ECHG70V-cFz7omdplXraYVjTvrZkkYqdaf0Z8-vnQ6NiLeOXW3GLCqnlYjabwf59RMaUv8",
                RefreshToken = "1/PN_s59ZaV_pPDvYjM-EDeUlxg9OHTh8gzdAGmgsERrM",
                ExpiresIn = 3600,
                Scope = "create",
                TokenType = "bearer"
            });

            return httpClient;
        }

        public GoogleControllerTests()
        {
            SecuritySettings securitySettings = new SecuritySettings()
            {
                SystemSettings = new SystemSettings
                {
                    FacebookAuthSettings = new FacebookAuthSettings
                    {
                        OAuthUrl = "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials",
                        UserTokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}"
                    },
                    GoogleAuthSettings = new GoogleAuthSettings
                    {
                        TokenUrl = "https://accounts.google.com/o/oauth2/token"
                    }
                },
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AppId = "your facebook app id",
                AppSecret = "your facebook app secret",
                GoogleSecuritySettings = new GoogleSecuritySettings
                {
                    APIKey = "<api key>",
                    ClientId = "<client id>",
                    ClientSecret = "<client secret>",
                    RedirectUri = "http://localhost/"
                }
            };

            this.SecuritySettings = securitySettings;

            this.MockGoogleClient = this.InitMockGoogleClient(this.SecuritySettings);
            this.MockHttpClient = this.InitMockHttpClient(this.SecuritySettings);
        }

        [Fact]
        public async Task Test_GoogleController_Pass()
        {
            //Arrange
            GoogleAuthModel googleAuthModel = new GoogleAuthModel
            {
                APIKey = "<api key>",
                AuthorizationCode = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            GoogleClient client = new GoogleClient(this.SecuritySettings, this.MockHttpClient.Object);

            GoogleAuthenticator authenticator = new GoogleAuthenticator(this.SecuritySettings.GoogleSecuritySettings,
                                                                        client);

            var controller = new GoogleController(authenticator);

            //Act
            var result = await controller.Create(googleAuthModel);

            var googleResponse = ((result as ObjectResult).Value as GoogleResponseModel);
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True(googleResponse.AccessToken.IsValidJwtToken());
            this.MockHttpClient.Verify(x => x.SendAsync<GoogleResponseModel>(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Fact]
        public async Task Test_GoogleController_GoogleAuth_Fail()
        {
            //Arrange

            //Google Client returns IsAuthenticated false
            this.MockGoogleClient = this.InitMockGoogleClient(this.SecuritySettings, false);

            GoogleAuthModel facebookAuthModel = new GoogleAuthModel
            {
                APIKey = "<api key>",
                AuthorizationCode = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            GoogleAuthenticator authenticator = new GoogleAuthenticator(this.SecuritySettings.GoogleSecuritySettings,
                                                                        this.MockGoogleClient.Object);

            var controller = new GoogleController(authenticator);

            //Act
            var result = await controller.Create(facebookAuthModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            this.MockGoogleClient.Verify(x => x.PostSecurityRequest(facebookAuthModel), Times.Once);
        }

        [Fact]
        public async Task Test_GoogleController_NoAPIKey_Fail()
        {
            //Arrange

            //API Key absent
            GoogleAuthModel facebookAuthModel = new GoogleAuthModel
            {
                AuthorizationCode = "<auth_code>"
            };

            GoogleClient client = new GoogleClient(this.SecuritySettings, this.MockHttpClient.Object);

            GoogleAuthenticator authenticator = new GoogleAuthenticator(this.SecuritySettings.GoogleSecuritySettings,
                                                                        client);

            var controller = new GoogleController(authenticator);

            try
            {
                //Act
                var result = await controller.Create(facebookAuthModel);
            }
            catch (SecurityException)
            {
                //Assert
                this.MockGoogleClient.Verify(x => x.PostSecurityRequest(facebookAuthModel), Times.Never);
            }
        }

        [Fact]
        public async Task Test_GoogleController_NoAuthorizationCode_Fail()
        {
            //Arrange

            //Authorization Code absent
            GoogleAuthModel googleAuthModel = new GoogleAuthModel
            {
                APIKey = "<api key>"
            };

            GoogleClient client = new GoogleClient(this.SecuritySettings, this.MockHttpClient.Object);

            GoogleAuthenticator authenticator = new GoogleAuthenticator(this.SecuritySettings.GoogleSecuritySettings,
                                                                        client);

            var controller = new GoogleController(authenticator);

            try
            {
                //Act
                var result = await controller.Create(googleAuthModel);
            }
            catch (SecurityException ex)
            {
                //Assert
                Assert.IsType<SecurityException>(ex);
                this.MockGoogleClient.Verify(x => x.PostSecurityRequest(googleAuthModel), Times.Never);
            }
        }

        [Fact]
        public async Task Test_GoogleController_GoogleAuthorizeAttribute_InvalidAPIKey_ReturnsUnauthorizedResult()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("securitySettings.json")
                .Build();

            // Arrange
            var server = new TestServer(new WebHostBuilder()
                                .UseConfiguration(config)
                                .UseStartup<Startup>());
            var client = server.CreateClient();
            var url = "/google";
            var expected = HttpStatusCode.Unauthorized;

            GoogleAuthModel googleAuthModel = new GoogleAuthModel
            {
                AuthorizationCode = "string",
                APIKey = "invalid api key"
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(googleAuthModel));

            // Act
            var response = await client.PostAsync(url, httpContent);            

            // Assert
            Assert.Equal(expected, response.StatusCode);

            //Arrange
            httpContent = new StringContent(string.Empty);

            // Act
            response = await client.PostAsync(url, httpContent);

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }

    }
}
