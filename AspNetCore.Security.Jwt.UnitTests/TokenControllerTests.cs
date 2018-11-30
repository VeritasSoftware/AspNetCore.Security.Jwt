using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class TokenControllerTests
    {
        [Fact]
        public async Task Test_TokenController_Default_Pass()
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

            var securityService = new SecurityService(securitySettings);

            //Authenticator returns a true, token is generated.
            var authenticator = new DefaultAuthenticator(true);

            var controller = new TokenController(securityService, authenticator);

            User user = new User
            {
                Id = "John Doe",
                Password = "xxxxxxxxx"
            };

            //Act
            var result = await controller.Create(user);

            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True((result as ObjectResult).Value.ToString().IsValidJwtToken());
        }

        [Fact]
        public async Task Test_TokenController_Default_Fail()
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

            var securityService = new SecurityService(securitySettings);

            //Authenticator returns a false, token is not generated.
            var authenticator = new DefaultAuthenticator(false);

            var controller = new TokenController(securityService, authenticator);

            User user = new User
            {
                Id = "John Doe",
                Password = "xxxxxxxxx"
            };

            //Act
            var result = await controller.Create(user);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Test_TokenController_CustomUserModel_Pass()
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

            var userModel = new UserModel
            {
                Id = "John Doe",
                Pwd = "xxxxxxxxx",
                Role = "xxx",
                DOB = DateTime.Now.ToShortDateString()
            };

            var securityService = new SecurityService<UserModel>(securitySettings, builder =>
            {
                builder.AddClaim(IdType.Name, model => model.Id)
                       .AddClaim(IdType.Role, model => model.Role)
                       .AddClaim("DOB", model => model.DOB);
            });

            //Authenticator returns a true, token is generated.
            var authenticator = new CustomAuthenticator(true);

            var controller = new TokenController<UserModel>(securityService, authenticator);

            //Act
            var result = await controller.Create(userModel);

            //Assert
            Assert.IsType<ObjectResult>(result);
            Assert.True((result as ObjectResult).Value.ToString().IsValidJwtToken());
        }

        [Fact]
        public async Task Test_TokenController_CustomUserModel_Fail()
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

            var userModel = new UserModel
            {
                Id = "John Doe",
                Pwd = "xxxxxxxxx",
                Role = "xxx",
                DOB = DateTime.Now.ToShortDateString()
            };

            var securityService = new SecurityService<UserModel>(securitySettings, builder =>
            {
                builder.AddClaim(IdType.Name, model => model.Id)
                       .AddClaim(IdType.Role, model => model.Role)
                       .AddClaim("DOB", model => model.DOB);
            });

            //Authenticator returns a false, token is not generated.
            var authenticator = new CustomAuthenticator(false);

            var controller = new TokenController<UserModel>(securityService, authenticator);

            //Act
            var result = await controller.Create(userModel);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
