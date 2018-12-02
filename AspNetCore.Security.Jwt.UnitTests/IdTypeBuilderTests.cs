using System;
using System.Linq;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class IdTypeBuilderTests
    {
        public IdTypeBuilderTests()
        {
            IdTypeHelpers.LoadClaimTypes();
        }

        [Fact]
        public void Test_IdTypeBuilder_Pass()
        {
            var userModel = new UserModel
            {
                Id = "John Doe"
            };

            var dob = DateTime.Now.ToShortDateString();

            var idTypeBuilder = new IdTypeBuilder<UserModel>(userModel);

            idTypeBuilder.AddClaim(IdType.Email, "JohnDoe@xyz.com")
                         .AddClaim("DOB", dob)
                         .AddClaim(IdType.Name, model => model.Id);

            var claims = idTypeBuilder.ToClaims();

            Assert.True(claims.Count == 3);
            Assert.True(claims.First().Value == "JohnDoe@xyz.com");
            Assert.True(claims.Skip(1).First().Value == dob);
            Assert.True(claims.Last().Value == userModel.Id);
        }
    }
}
