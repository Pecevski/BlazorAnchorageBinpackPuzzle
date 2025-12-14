namespace BlazorAnchorageBinpackPuzzle.Services.Interfaces;

using BlazorAnchorageBinpackPuzzle.Models;

public interface IFleetService
{
    /// <summary>
    /// Retrieves a random fleet configuration from the ESA API.
    /// </summary>
    /// <returns>Fleet response with anchorage size and vessel types.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API call fails.</exception>
    Task<FleetResponse> GetRandomFleetAsync();
}