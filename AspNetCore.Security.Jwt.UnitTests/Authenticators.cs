using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt.UnitTests
{
    class DefaultAuthenticator : IAuthentication
    {
        bool returnValue;

        public DefaultAuthenticator()
        {

        }

        public DefaultAuthenticator(bool returnValue)
        {
            this.returnValue = returnValue;
        }

        public async Task<bool> IsValidUser(string id, string password)
        {
            return this.returnValue;
        }
    }

    class CustomAuthenticator : IAuthentication<UserModel>
    {
        bool returnValue;

        public CustomAuthenticator(bool returnValue)
        {
            this.returnValue = returnValue;
        }

        public async Task<bool> IsValidUser(UserModel user)
        {
            return this.returnValue;
        }
    }
}
