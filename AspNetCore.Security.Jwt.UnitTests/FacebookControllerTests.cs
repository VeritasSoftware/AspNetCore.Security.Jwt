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
        private SecuritySettings SecuritySettings { get; set; }

        internal Mock<FacebookClient> InitMockFacebookClient(SecuritySettings securitySettings, bool isAuthenticated = true)
        {
            var securityClient = new Mock<FacebookClient>(securitySettings);
            securityClient.Setup(x => x.PostSecurityRequest(It.IsAny<FacebookAuthModel>())).ReturnsAsync(() => isAuthenticated);

            return securityClient;
        }

        public FacebookControllerTests()
        {
            SecuritySettings securitySettings = new SecuritySettings()
            {
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
        }

        [Fact]
        public async Task Test_FacebookController_Pass()
        {
            //Arrange
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
            Assert.IsType<ObjectResult>(result);
            Assert.True((result as ObjectResult).Value.ToString().IsValidJwtToken());
            this.MockFacebookClient.Verify(x => x.PostSecurityRequest(facebookAuthModel), Times.Once);
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
