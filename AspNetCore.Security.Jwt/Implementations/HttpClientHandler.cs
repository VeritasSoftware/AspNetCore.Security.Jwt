using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public class HttpClientHandler : IHttpClient
    {
        public virtual async Task<TResponse> SendAsync<TResponse>(string uri, HttpRequestMessage request)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseStr = await response.Content.ReadAsStringAsync();

                var responseObj = JsonConvert.DeserializeObject<TResponse>(responseStr);

                return responseObj;
            }
        }
    }
}
