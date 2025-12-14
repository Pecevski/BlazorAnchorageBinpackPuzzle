using BlazorAnchorageBinpackPuzzle;
using BlazorAnchorageBinpackPuzzle.Services;
using BlazorAnchorageBinpackPuzzle.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load API settings from appsettings.json
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
using var response = await httpClient.GetAsync("appsettings.json");
using var content = await response.Content.ReadAsStreamAsync();

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var settings = await JsonSerializer.DeserializeAsync<ApiSettings>(content, options) 
    ?? new ApiSettings();

// Configure HttpClient for ESA API
builder.Services.AddHttpClient<IFleetService, FleetService>(client =>
{
    client.BaseAddress = new Uri(settings.EsaApiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register domain logic
builder.Services.AddScoped<IAnchoragePlanner, AnchoragePlanner>();

await builder.Build().RunAsync();
