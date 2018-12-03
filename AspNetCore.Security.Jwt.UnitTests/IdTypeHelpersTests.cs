using System.Linq;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class IdTypeHelpersTests
    {
        [Fact]
        public void Test_IdTypeHelpers_Pass()
        {
            IdTypeHelpers.ClaimTypes?.Clear();

            IdTypeHelpers.LoadClaimTypes();

            Assert.True(IdTypeHelpers.ClaimTypes.Any());
        }
    }
}
