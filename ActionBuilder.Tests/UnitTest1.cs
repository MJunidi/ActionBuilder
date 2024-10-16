using ActionBuilder.Controllers;
using ActionBuilder.Models;
using Labiba.Actions.Logger.Core.Models;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ActionBuilder.Tests
{
    public class UnitTest1
    {

        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly Mock<ITimer> _timerMock;
        private readonly Mock<HttpMessageHandler> _handlerMock;

        public UnitTest1()
        {
            _clientFactoryMock = new Mock<IHttpClientFactory>();
            _timerMock = new Mock<ITimer>();
            _handlerMock = new Mock<HttpMessageHandler>();
        }

        [Fact]
        
        public async Task CallAsync_ShouldReturnSucess_WhenApiResponseIsValid()
        {
            var request = new HttpClientBaseRequest
            {
                Method = HttpMethod.Get,
                Url = "https://jsonplaceholder.typicode.com/posts"
            };

            var logDetails = new LogDetails();


            var expectedResponse = "Expected API response";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponse),
            };

            // Mock HttpClient
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(_handlerMock.Object);
            _clientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var httpClientBase = new HomeController(_clientFactoryMock.Object);

            // Act
            var result = await httpClientBase.CallAsync<string>(request, logDetails);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Data);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.Equal(expectedResponse, logDetails.ResponseFromApi);

        }
    }
}