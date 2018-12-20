using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public interface IHttpClient
    {
        Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request);

        Task<TResponse> GetStringAsync<TResponse>(string uri);
    }
}
