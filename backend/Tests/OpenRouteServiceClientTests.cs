using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace Tests;

[TestFixture]
public class OpenRouteServiceClientTests
{
    private Mock<HttpMessageHandler> _mockHttpHandler;
    private HttpClient _httpClient;
    private Mock<IConfiguration> _mockConfig;
    private IOpenRouteServiceClient _orsClient;

    [SetUp]
    public void SetUp()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api.openrouteservice.org/")
        };

        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["OpenRouteService:ApiKey"]).Returns("test-api-key-12345");

        _orsClient = new OpenRouteServiceClient(_httpClient, _mockConfig.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task GetRouteAsync_ValidDrivingRoute_ReturnsRouteResult()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": [{""properties"": {""summary"": {""distance"": 150000, ""duration"": 5400}}}]}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Distance, Is.EqualTo(150.0));
        Assert.That(result.EstimatedTime, Is.EqualTo(1.5));
    }

    [Test]
    public async Task GetRouteAsync_ValidCyclingRoute_ReturnsRouteResult()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": [{""properties"": {""summary"": {""distance"": 50000, ""duration"": 7200}}}]}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Cycling);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Distance, Is.EqualTo(50.0));
    }

    [Test]
    public async Task GetRouteAsync_ValidWalkingRoute_ReturnsRouteResult()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": [{""properties"": {""summary"": {""distance"": 25000, ""duration"": 18000}}}]}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Walking);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Distance, Is.EqualTo(25.0));
    }

    [Test]
    public async Task GetRouteAsync_ApiReturnsError_ReturnsNull()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Invalid parameters")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Invalid", "Locations", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_ApiReturns401_ReturnsNull()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_EmptyFeaturesArray_ReturnsNull()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": []}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_NetworkTimeout_ReturnsNull()
    {
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new TaskCanceledException());

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_HttpRequestException_ReturnsNull()
    {
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_UnexpectedException_ReturnsNull()
    {
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new Exception("Unexpected error"));

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetRouteAsync_ConvertKmAndHours_CorrectlyCalculates()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": [{""properties"": {""summary"": {""distance"": 300500, ""duration"": 10800}}}]}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result!.Distance, Is.EqualTo(300.5).Within(0.01));
        Assert.That(result.EstimatedTime, Is.EqualTo(3.0).Within(0.01));
    }

    [Test]
    public async Task GetRouteAsync_VerifiesApiKeyInUrl()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{""features"": [{""properties"": {""summary"": {""distance"": 100000, ""duration"": 3600}}}]}")
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("api_key=test-api-key-12345")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        var result = await _orsClient.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving);

        Assert.That(result, Is.Not.Null);
    }
}