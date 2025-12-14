namespace BlazorAnchorageBinpackPuzzle.Tests.Services;

using BlazorAnchorageBinpackPuzzle.Models;
using BlazorAnchorageBinpackPuzzle.Services;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Xunit;

/// <summary>
/// Unit tests for the FleetService class.
/// Tests API communication, response deserialization, and error handling.
/// </summary>
public class FleetServiceTests : IAsyncLifetime
{
    private HttpClient _httpClient = null!;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
    private FleetService _fleetService = null!;

    public async Task InitializeAsync()
    {
        // Setup mock HttpMessageHandler for testing
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://esa.instech.no")
        };
        _fleetService = new FleetService(_httpClient);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        await Task.CompletedTask;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidHttpClient_Succeeds()
    {
        // Arrange
        var httpClient = new HttpClient { BaseAddress = new Uri("https://esa.instech.no") };

        // Act
        var service = new FleetService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FleetService(null!));
    }

    #endregion

    #region GetRandomFleetAsync - Successful Response Tests

    [Fact]
    public async Task GetRandomFleetAsync_WithValidResponse_ReturnsFleetResponse()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(12, 15),
            new[]
            {
                new VesselType(new VesselDimensions(6, 5), "LNG Unit", 2),
                new VesselType(new VesselDimensions(3, 12), "Science & Engineering Ship", 5)
            });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12, result.AnchorageSize.Width);
        Assert.Equal(15, result.AnchorageSize.Height);
        Assert.Equal(2, result.Fleets.Length);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithValidResponse_DeserializesFleetDataCorrectly()
    {
        // Arrange
        var expectedFleetResponse = new FleetResponse(
            new AnchorageSize(32, 19),
            new[]
            {
                new VesselType(new VesselDimensions(6, 12), "Tug Engineering Ship", 10),
                new VesselType(new VesselDimensions(5, 10), "Science & Engineering Ship", 13),
                new VesselType(new VesselDimensions(4, 1), "Ultrasonic Strategic Hexamaran with Scuba equipment", 3),
                new VesselType(new VesselDimensions(6, 30), "LNG Unit with Underwater Monitoring", 3),
                new VesselType(new VesselDimensions(5, 1), "Rare Hexamaran with Public WC", 2)
            });

        var json = JsonSerializer.Serialize(expectedFleetResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(32, result.AnchorageSize.Width);
        Assert.Equal(19, result.AnchorageSize.Height);
        Assert.Equal(5, result.Fleets.Length);

        // Verify first fleet type
        Assert.Equal("Tug Engineering Ship", result.Fleets[0].ShipDesignation);
        Assert.Equal(6, result.Fleets[0].SingleShipDimensions.Width);
        Assert.Equal(12, result.Fleets[0].SingleShipDimensions.Height);
        Assert.Equal(10, result.Fleets[0].ShipCount);

        // Verify last fleet type
        Assert.Equal("Rare Hexamaran with Public WC", result.Fleets[4].ShipDesignation);
        Assert.Equal(5, result.Fleets[4].SingleShipDimensions.Width);
        Assert.Equal(1, result.Fleets[4].SingleShipDimensions.Height);
        Assert.Equal(2, result.Fleets[4].ShipCount);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithMultipleVessels_DeserializesAllVessels()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(20, 20),
            new[]
            {
                new VesselType(new VesselDimensions(3, 4), "Vessel 1", 1),
                new VesselType(new VesselDimensions(5, 6), "Vessel 2", 2),
                new VesselType(new VesselDimensions(7, 8), "Vessel 3", 3),
                new VesselType(new VesselDimensions(2, 2), "Vessel 4", 4)
            });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.Equal(4, result.Fleets.Length);
        Assert.Equal(1 + 2 + 3 + 4, result.Fleets.Sum(f => f.ShipCount)); // Total 10 vessels
    }

    #endregion

    #region GetRandomFleetAsync - Error Handling Tests

    [Fact]
    public async Task GetRandomFleetAsync_With404NotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fleetService.GetRandomFleetAsync());
        Assert.Contains("Failed to fetch fleet data from ESA API", exception.Message);
    }

    [Fact]
    public async Task GetRandomFleetAsync_With500ServerError_ThrowsInvalidOperationException()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fleetService.GetRandomFleetAsync());
        Assert.Contains("Failed to fetch fleet data from ESA API", exception.Message);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithNullResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fleetService.GetRandomFleetAsync());
        Assert.Contains("Empty response from fleet API", exception.Message);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithNetworkTimeout_ThrowsInvalidOperationException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network timeout"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fleetService.GetRandomFleetAsync());
        Assert.Contains("Failed to fetch fleet data from ESA API", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithInvalidJson_ThrowsInvalidOperationException()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _fleetService.GetRandomFleetAsync());
        Assert.Contains("Failed to fetch fleet data from ESA API", exception.Message);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithEmptyFleets_ReturnsResponseWithEmptyFleets()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(10, 10),
            Array.Empty<VesselType>());

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Fleets);
        Assert.Equal(10, result.AnchorageSize.Width);
    }

    #endregion

    #region Request Verification Tests

    [Fact]
    public async Task GetRandomFleetAsync_SendsGetRequestToCorrectEndpoint()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(12, 15),
            new[] { new VesselType(new VesselDimensions(6, 5), "Test Ship", 2) });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.AbsoluteUri.Contains("api/fleets/random")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.NotNull(result);
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri!.AbsoluteUri.Contains("api/fleets/random")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetRandomFleetAsync_MakesExactlyOneHttpCall()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(12, 15),
            new[] { new VesselType(new VesselDimensions(6, 5), "Test Ship", 2) });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        await _fleetService.GetRandomFleetAsync();

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetRandomFleetAsync_WithLargeAnchorageSize_DeserializesCorrectly()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(1000, 1000),
            new[] { new VesselType(new VesselDimensions(500, 500), "Massive Ship", 1) });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.Equal(1000, result.AnchorageSize.Width);
        Assert.Equal(1000, result.AnchorageSize.Height);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithLongVesselNames_DeserializesCorrectly()
    {
        // Arrange
        var longName = "Ultrasonic Strategic Hexamaran with Scuba equipment and Advanced Navigation Systems";
        var validResponse = new FleetResponse(
            new AnchorageSize(12, 15),
            new[] { new VesselType(new VesselDimensions(6, 5), longName, 2) });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.Equal(longName, result.Fleets[0].ShipDesignation);
    }

    [Fact]
    public async Task GetRandomFleetAsync_WithZeroShipCount_DeserializesCorrectly()
    {
        // Arrange
        var validResponse = new FleetResponse(
            new AnchorageSize(12, 15),
            new[] { new VesselType(new VesselDimensions(6, 5), "Ghost Ship", 0) });

        var json = JsonSerializer.Serialize(validResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _fleetService.GetRandomFleetAsync();

        // Assert
        Assert.Equal(0, result.Fleets[0].ShipCount);
    }

    #endregion
}