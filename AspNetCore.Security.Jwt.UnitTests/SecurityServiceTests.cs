using System;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class SecurityServiceTests
    {
        [Fact]
        public void Test_SecurityService_Default_Pass()
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

            //Act
            var token = securityService.GenerateToken("John Doe");

            //Assert
            Assert.NotEmpty(token);
            Assert.True(token.IsValidJwtToken());
        }

        [Fact]
        public void Test_SecurityService_CustomUserModel_Pass()
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

            //Act
            var token = securityService.GenerateToken(userModel);

            //Assert
            Assert.NotEmpty(token);
            Assert.True(token.IsValidJwtToken());
        }
    }
}
