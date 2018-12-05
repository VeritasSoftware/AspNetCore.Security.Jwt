using AspNetCore.Security.Jwt.Facebook;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class FacebookControllerTests
    {
        private Mock<FacebookClient> MockFacebookClient { get; set; }
        private Mock<HttpClientHandler> MockHttpClient { get; set; }
        private SecuritySettings SecuritySettings { get; set; }

        internal Mock<FacebookClient> InitMockFacebookClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var securityClient = new Mock<FacebookClient>(securitySettings, It.IsAny<HttpClientHandler>());
            securityClient.Setup(x => x.PostSecurityRequest(It.IsAny<FacebookAuthModel>())).ReturnsAsync(() => isAuthenticated);

            return securityClient;
        }

        internal Mock<HttpClientHandler> InitMockHttpClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var httpClient = new Mock<HttpClientHandler>();
            httpClient.Setup(x => x.GetStringAsync<FacebookAppAccessToken>(It.IsAny<string>())).ReturnsAsync(() => new FacebookAppAccessToken
            {
                AccessToken = "ya29.GltoBsrJ_RRZzI-DEzU3l6nDz_qwy7RFM-zFv7MA1z6ZeU3IijEZa_ECHG70V-cFz7omdplXraYVjTvrZkkYqdaf0Z8-vnQ6NiLeOXW3GLCqnlYjabwf59RMaUv8",
                TokenType = "bearer"
            });
            httpClient.Setup(x => x.GetStringAsync<FacebookUserAccessTokenValidation>(It.IsAny<string>())).ReturnsAsync(() => new FacebookUserAccessTokenValidation
            {
                Data = new FacebookUserAccessTokenData
                {
                    IsValid = isAuthenticated
                }
            });


            return httpClient;
        }

        public FacebookControllerTests()
        {
            SecuritySettings securitySettings = new SecuritySettings()
            {
                AuthSettings = new AuthSettings
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
                AppSecret = "your facebook app secret"
            };

            this.SecuritySettings = securitySettings;

            this.MockFacebookClient = this.InitMockFacebookClient(this.SecuritySettings);
            this.MockHttpClient = this.InitMockHttpClient(this.SecuritySettings);
        }

        [Fact]
        public async Task Test_FacebookController_Pass()
        {
            //Arrange
            FacebookAuthModel facebookAuthModel = new FacebookAuthModel
            {
                UserAccessToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            var client = new FacebookClient(this.SecuritySettings, this.MockHttpClient.Object);

            FacebookAuthenticator authenticator = new FacebookAuthenticator(client);

            var securityService = new SecurityService<FacebookAuthModel>(this.SecuritySettings);

            var controller = new FacebookController(securityService, authenticator);

            //Act
            var result = await controller.Create(facebookAuthModel);

            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True((result as ObjectResult).Value.ToString().IsValidJwtToken());
            this.MockHttpClient.Verify(x => x.GetStringAsync<FacebookAppAccessToken>(It.IsAny<string>()), Times.Once);
            this.MockHttpClient.Verify(x => x.GetStringAsync<FacebookUserAccessTokenValidation>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Test_FacebookController_FacebookAuth_Fail()
        {
            //Arrange

            //Facebook Client returns IsAuthenticated false
            this.MockFacebookClient = this.InitMockFacebookClient(this.SecuritySettings, false);

            FacebookAuthModel facebookAuthModel = new FacebookAuthModel
            {
                UserAccessToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            FacebookAuthenticator authenticator = new FacebookAuthenticator(this.MockFacebookClient.Object);

            var securityService = new SecurityService<FacebookAuthModel>(this.SecuritySettings);

            var controller = new FacebookController(securityService, authenticator);

            //Act
            var result = await controller.Create(facebookAuthModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            this.MockFacebookClient.Verify(x => x.PostSecurityRequest(facebookAuthModel), Times.Once);
        }

        [Fact]
        public async Task Test_FacebookController_NoUserToken_Fail()
        {
            //Arrange

            //Facebook User Token absent
            FacebookAuthModel facebookAuthModel = new FacebookAuthModel();

            FacebookAuthenticator authenticator = new FacebookAuthenticator(this.MockFacebookClient.Object);

            var securityService = new SecurityService<FacebookAuthModel>(this.SecuritySettings);

            var controller = new FacebookController(securityService, authenticator);

            try
            {
                //Act
                var result = await controller.Create(facebookAuthModel);
            }
            catch(SecurityException ex)
            {
                //Assert
                Assert.IsType<SecurityException>(ex);
                this.MockFacebookClient.Verify(x => x.PostSecurityRequest(facebookAuthModel), Times.Never);
            }                        
        }
    }
}
