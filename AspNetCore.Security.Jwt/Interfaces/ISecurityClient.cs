using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    internal interface ISecurityClient<TResponse>
    {
        Task<TResponse> PostSecurityRequest();
    }

    internal interface ISecurityClient<TRequest, TResponse>
    {
        Task<TResponse> PostSecurityRequest(TRequest request);
    }
}
