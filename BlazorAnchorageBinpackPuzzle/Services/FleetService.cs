namespace BlazorAnchorageBinpackPuzzle.Services;

using BlazorAnchorageBinpackPuzzle.Models;
using BlazorAnchorageBinpackPuzzle.Services.Interfaces;
using System.Net.Http.Json;

public class FleetService : IFleetService
{
    private readonly HttpClient _httpClient;

    public FleetService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<FleetResponse> GetRandomFleetAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<FleetResponse>("api/fleets/random");
            return response ?? throw new InvalidOperationException("Empty response from fleet API");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Failed to fetch fleet data from ESA API", ex);
        }
    }
}
