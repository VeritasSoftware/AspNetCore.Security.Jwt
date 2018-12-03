using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class IdTypeClaimTypesExtensionsTests
    {
        [Fact]
        public void Test_IdTypeClaimTypesExtensions_Pass()
        {
            IdTypeHelpers.LoadClaimTypes();

            var nameClaimTypes = IdType.Name.ToClaimTypes();

            Assert.True(nameClaimTypes == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        }

        [Fact]
        public void Test_IdTypeClaimTypesExtensions_ClaimTypesDictionaryEmpty_Pass()
        {
            IdTypeHelpers.ClaimTypes?.Clear();

            var nameClaimTypes = IdType.Name.ToClaimTypes();

            Assert.True(nameClaimTypes == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        }

    }
}
