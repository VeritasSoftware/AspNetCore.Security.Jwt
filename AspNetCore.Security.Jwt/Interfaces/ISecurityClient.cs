using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    internal interface ISecurityClient<TResponse>
    {
        Task<TResponse> PostSecurityRequest();
    }
}
