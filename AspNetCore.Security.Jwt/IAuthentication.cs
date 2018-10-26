namespace AspNetCore.Security.Jwt
{
    using System.Threading.Tasks;

    public interface IAuthentication
    {
        Task<bool> IsValidUser(string id, string password);        
    }

    public interface IAuthentication<TUserModel>
        where TUserModel : class, IAuthenticationUser
    {
        Task<bool> IsValidUser(TUserModel user);
    }
}
