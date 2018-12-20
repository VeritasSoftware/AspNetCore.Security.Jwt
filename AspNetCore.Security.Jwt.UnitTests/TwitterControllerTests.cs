using AspNetCore.Security.Jwt;
using AspNetCore.Security.Jwt.Twitter;
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
    public class TwitterControllerTests
    {
        private Mock<TwitterClient> MockTwitterClient { get; set; }
        private Mock<HttpClientHandler> MockHttpClient { get; set; }
        private SecuritySettings SecuritySettings { get; set; }

        internal Mock<TwitterClient> InitMockTwitterClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var securityClient = new Mock<TwitterClient>(securitySettings, It.IsAny<HttpClientHandler>());
            securityClient.Setup(x => x.PostSecurityRequest()).ReturnsAsync(() => new TwitterResponseModel
            {
                IsAuthenticated = isAuthenticated,
                AccessToken = "ya29.GltoBsrJ_RRZzI-DEzU3l6nDz_qwy7RFM-zFv7MA1z6ZeU3IijEZa_ECHG70V-cFz7omdplXraYVjTvrZkkYqdaf0Z8-vnQ6NiLeOXW3GLCqnlYjabwf59RMaUv8"
            });

            return securityClient;
        }

        internal Mock<HttpClientHandler> InitMockHttpClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var httpClient = new Mock<HttpClientHandler>();
            httpClient.Setup(x => x.SendAsync<TwitterResponseModel>(It.IsAny<HttpRequestMessage>())).ReturnsAsync(() => new TwitterResponseModel
            {
                AccessToken = "ya29.GltoBsrJ_RRZzI-DEzU3l6nDz_qwy7RFM-zFv7MA1z6ZeU3IijEZa_ECHG70V-cFz7omdplXraYVjTvrZkkYqdaf0Z8-vnQ6NiLeOXW3GLCqnlYjabwf59RMaUv8"
            });

            return httpClient;
        }

        public TwitterControllerTests()
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
                    TwitterAuthSettings = new TwitterAuthSettings
                    {
                        TokenUrl = "https://accounts.twitter.com/o/oauth2/token"
                    }
                },
                Secret = "a secret that needs to be at least 16 characters long",
                Issuer = "your app",
                Audience = "the client of your app",
                IdType = IdType.Name,
                TokenExpiryInHours = 1.2,
                AppId = "your facebook app id",
                AppSecret = "your facebook app secret",
                TwitterSecuritySettings = new TwitterSecuritySettings
                {
                    APIKey = "<api key>",
                    ConsumerKey = "<client id>",
                    ConsumerSecret = "<client secret>"
                }
            };

            this.SecuritySettings = securitySettings;

            this.MockTwitterClient = this.InitMockTwitterClient(this.SecuritySettings);
            this.MockHttpClient = this.InitMockHttpClient(this.SecuritySettings);
        }

        [Fact]
        public async Task Test_TwitterController_Pass()
        {
            //Arrange
            TwitterAuthModel twitterAuthModel = new TwitterAuthModel
            {
                APIKey = "<api key>"
            };

            TwitterClient client = new TwitterClient(this.SecuritySettings, this.MockHttpClient.Object);

            TwitterAuthenticator authenticator = new TwitterAuthenticator(this.SecuritySettings.TwitterSecuritySettings,
                                                                        client);

            var controller = new TwitterController(authenticator);

            //Act
            var result = await controller.Create(twitterAuthModel);

            var twitterAccessToken = ((result as ObjectResult).Value as string);
            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True(twitterAccessToken.IsValidJwtToken());
            this.MockHttpClient.Verify(x => x.SendAsync<TwitterResponseModel>(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Fact]
        public async Task Test_TwitterController_TwitterAuth_Fail()
        {
            //Arrange

            //Twitter Client returns IsAuthenticated false
            this.MockTwitterClient = this.InitMockTwitterClient(this.SecuritySettings, false);

            TwitterAuthModel facebookAuthModel = new TwitterAuthModel
            {
                APIKey = "<api key>"
            };

            TwitterAuthenticator authenticator = new TwitterAuthenticator(this.SecuritySettings.TwitterSecuritySettings,
                                                                        this.MockTwitterClient.Object);

            var controller = new TwitterController(authenticator);

            //Act
            var result = await controller.Create(facebookAuthModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            this.MockTwitterClient.Verify(x => x.PostSecurityRequest(), Times.Once);
        }

        [Fact]
        public async Task Test_TwitterController_NoAPIKey_Fail()
        {
            //Arrange

            //API Key absent
            TwitterAuthModel facebookAuthModel = new TwitterAuthModel();

            TwitterClient client = new TwitterClient(this.SecuritySettings, this.MockHttpClient.Object);

            TwitterAuthenticator authenticator = new TwitterAuthenticator(this.SecuritySettings.TwitterSecuritySettings,
                                                                        client);

            var controller = new TwitterController(authenticator);

            try
            {
                //Act
                var result = await controller.Create(facebookAuthModel);
            }
            catch (SecurityException)
            {
                //Assert
                this.MockTwitterClient.Verify(x => x.PostSecurityRequest(), Times.Never);
            }
        }

        [Fact]
        public async Task Test_TwitterController_NoAuthorizationCode_Fail()
        {
            //Arrange

            //Authorization Code absent
            TwitterAuthModel twitterAuthModel = new TwitterAuthModel
            {
                APIKey = "<api key>"
            };

            TwitterClient client = new TwitterClient(this.SecuritySettings, this.MockHttpClient.Object);

            TwitterAuthenticator authenticator = new TwitterAuthenticator(this.SecuritySettings.TwitterSecuritySettings,
                                                                        client);

            var controller = new TwitterController(authenticator);

            try
            {
                //Act
                var result = await controller.Create(twitterAuthModel);
            }
            catch (SecurityException ex)
            {
                //Assert
                Assert.IsType<SecurityException>(ex);
                this.MockTwitterClient.Verify(x => x.PostSecurityRequest(), Times.Never);
            }
        }

        [Fact]
        public async Task Test_TwitterController_TwitterAuthorizeAttribute_InvalidAPIKey_ReturnsUnauthorizedResult()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("securitySettings.json")
                .Build();

            // Arrange
            var server = new TestServer(new WebHostBuilder()
                                .UseConfiguration(config)
                                .UseStartup<Startup>());
            var client = server.CreateClient();
            var url = "/twitter";
            var expected = HttpStatusCode.Unauthorized;

            TwitterAuthModel twitterAuthModel = new TwitterAuthModel
            {
                APIKey = "invalid api key"
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(twitterAuthModel));

            // Act
            var response = await client.PostAsync(url, httpContent);

            // Assert
            Assert.Equal(expected, response.StatusCode);

            //Arrange
            httpContent = new StringContent(string.Empty);

            // Act
            response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }
    }
}
