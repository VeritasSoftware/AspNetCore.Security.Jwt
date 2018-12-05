using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public interface IHttpClient
    {
        Task<TResponse> SendAsync<TResponse>(string uri, HttpRequestMessage request);

        Task<string> GetStringAsync(string uri);

        Task<TResponse> GetStringAsync<TResponse>(string uri);
    }
}
