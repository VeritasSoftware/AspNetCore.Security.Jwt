using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public interface IHttpClient
    {
        Task<TResponse> SendAsync<TResponse>(string uri, HttpRequestMessage request);
    }
}
