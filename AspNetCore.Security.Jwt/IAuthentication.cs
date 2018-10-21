namespace AspNetCore.Security.Jwt
{
    public interface IAuthentication
    {
        bool IsValidUser(string userName, string password);
    }
}
