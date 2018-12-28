using AspNetCore.Security.Jwt.Twitter;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Security.Jwt.UnitTests
{
    public class HttpClientHandlerTests
    {
        [Fact]
        public async Task Test_HttpClientHandler_SendAsync_Pass()
        {
            TwitterResponseModel twitterResponseModel = new TwitterResponseModel()
            {
                AccessToken = "AAAAAAAAAAAAAAAAAAAAAKYxUgAAAAAAvt5RRnHfJOrJa0aFQxt1iyZjQgs%3DtmlrfKDW602zOUNchylCZ9k2oJbkUnIL0hzsA2Tr8qPICj1hG6",
                IsAuthenticated = false
            };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(twitterResponseModel));

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                                    ItExpr.IsAny<HttpRequestMessage>(),
                                                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            HttpClientHandler httpClientHandler = new HttpClientHandler(httpClient);

            var expectedUri = new Uri("http://cnn.com");

            var response = await httpClientHandler.SendAsync<TwitterResponseModel>(new HttpRequestMessage(HttpMethod.Post, expectedUri));

            Assert.True(response.AccessToken == "AAAAAAAAAAAAAAAAAAAAAKYxUgAAAAAAvt5RRnHfJOrJa0aFQxt1iyZjQgs%3DtmlrfKDW602zOUNchylCZ9k2oJbkUnIL0hzsA2Tr8qPICj1hG6");
            handlerMock.Protected().Verify(
                        "SendAsync",
                        Times.Exactly(1), // we expected a single external request
                        ItExpr.Is<HttpRequestMessage>(req =>
                            req.Method == HttpMethod.Post  // we expected a GET request
                            && req.RequestUri == expectedUri // to this uri
                        ),
                        ItExpr.IsAny<CancellationToken>()
                    );


        }

    }
}
