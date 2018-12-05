using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public class HttpClientHandler : IHttpClient
    {
        public virtual async Task<string> GetStringAsync(string uri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(uri);
            }
        }

        public virtual async Task<TResponse> GetStringAsync<TResponse>(string uri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var reponseStr = await httpClient.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<TResponse>(reponseStr);
            }
        }

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
