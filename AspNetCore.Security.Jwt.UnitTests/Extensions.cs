using System.Text.RegularExpressions;

namespace AspNetCore.Security.Jwt.UnitTests
{
    static class Extensions
    {
        const string JWT_REGEX = @"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$";

        public static bool IsValidJwtToken(this string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return false;
            }
            return Regex.Match(jwt, JWT_REGEX).Success;
        }
    }
}
