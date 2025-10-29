
using ClienteApi.Application.DTOs.ViaCep;
using ClienteApi.Infrastructure.ExternalServices;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ClienteApi.Infrastructure.Tests.ExternalServices
{
    public class ViaCepServiceTests
    {
        private readonly Mock<ILogger<ViaCepService>> _loggerMock;

        public ViaCepServiceTests()
        {
            _loggerMock = new Mock<ILogger<ViaCepService>>();
        }

        private HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };
        }

        [Fact]
        public async Task GetAddressByCepAsync_Should_ReturnAddress_WhenCepIsValidAndApiReturnsSuccess()
        {
            // Arrange
            var cep = "01001-000";
            var expectedResponse = new ViaCepResponse { Cep = cep, Logradouro = "Praça da Sé", Localidade = "São Paulo" };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            };
            var httpClient = CreateMockHttpClient(responseMessage);
            var service = new ViaCepService(httpClient, _loggerMock.Object);

            // Act
            var result = await service.GetAddressByCepAsync(cep);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetAddressByCepAsync_Should_ReturnNull_WhenApiReturnsError()
        {
            // Arrange
            var cep = "99999-999";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };
            var httpClient = CreateMockHttpClient(responseMessage);
            var service = new ViaCepService(httpClient, _loggerMock.Object);

            // Act
            var result = await service.GetAddressByCepAsync(cep);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAddressByCepAsync_Should_ReturnNull_WhenCepIsNotFoundByApi()
        {
            // Arrange
            var cep = "00000-000";
            var apiResponse = new { erro = true };
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(apiResponse))
            };
            var httpClient = CreateMockHttpClient(responseMessage);
            var service = new ViaCepService(httpClient, _loggerMock.Object);

            // Act
            var result = await service.GetAddressByCepAsync(cep);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("abcde-fgh")]      
        [InlineData("")]
        public async Task GetAddressByCepAsync_Should_ReturnNull_ForInvalidCepFormat(string invalidCep)
        {
            // Arrange            
            var responseMessage = new HttpResponseMessage();
            var httpClient = CreateMockHttpClient(responseMessage);
            var service = new ViaCepService(httpClient, _loggerMock.Object);

            // Act
            var result = await service.GetAddressByCepAsync(invalidCep);

            // Assert
            result.Should().BeNull();
        }
    }
}
