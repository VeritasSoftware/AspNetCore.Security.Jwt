namespace AspNetCore.Security.Jwt
{
    using System.Threading.Tasks;

    public interface IAuthentication
    {
        Task<bool> IsValidUser(string userName, string password);
    }
}
