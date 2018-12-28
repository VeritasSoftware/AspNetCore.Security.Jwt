using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Security.Jwt
{
    public class HttpClientHandler : IHttpClient, IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (this.httpClient != null)
            {
                this.httpClient.Dispose();
            }
        }

        private readonly HttpClient httpClient;

        public HttpClientHandler(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public virtual async Task<TResponse> GetStringAsync<TResponse>(string uri)
        {
            using (this.httpClient)
            {
                var reponseStr = await httpClient.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<TResponse>(reponseStr);
            }
        }

        public virtual async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request)
        {
            using (this.httpClient)
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
