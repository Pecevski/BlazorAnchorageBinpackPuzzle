using BlazorAnchorageBinpackPuzzle;
using BlazorAnchorageBinpackPuzzle.Services;
using BlazorAnchorageBinpackPuzzle.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load appsettings.json then appsettings.{Environment}.json from wwwroot and merge ApiSettings
var httpClientForConfig = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

var apiSettings = new ApiSettings(); // default fallback

try
{
    // load base appsettings.json
    var baseResp = await httpClientForConfig.GetAsync("appsettings.json");
    if (baseResp.IsSuccessStatusCode)
    {
        await using var baseStream = await baseResp.Content.ReadAsStreamAsync();
        var baseDoc = await JsonSerializer.DeserializeAsync<JsonElement>(baseStream, jsonOptions);
        if (baseDoc.ValueKind == JsonValueKind.Object && baseDoc.TryGetProperty("ApiSettings", out var apiSection))
        {
            var parsed = apiSection.Deserialize<ApiSettings>(jsonOptions);
            if (parsed != null) apiSettings = parsed;
        }
    }

    // load environment-specific appsettings.{Environment}.json and override
    var env = builder.HostEnvironment.Environment;
    var envFileName = $"appsettings.{env}.json";
    var envResp = await httpClientForConfig.GetAsync(envFileName);
    if (envResp.IsSuccessStatusCode)
    {
        await using var envStream = await envResp.Content.ReadAsStreamAsync();
        var envDoc = await JsonSerializer.DeserializeAsync<JsonElement>(envStream, jsonOptions);
        if (envDoc.ValueKind == JsonValueKind.Object && envDoc.TryGetProperty("ApiSettings", out var envApiSection))
        {
            var parsedEnv = envApiSection.Deserialize<ApiSettings>(jsonOptions);
            if (parsedEnv != null && !string.IsNullOrWhiteSpace(parsedEnv.EsaApiBaseUrl))
            {
                apiSettings.EsaApiBaseUrl = parsedEnv.EsaApiBaseUrl;
            }
        }
    }
}
catch
{
    // ignore config load failures — keep defaults
}

// Ensure we have a sensible base URL. If it's relative (starts with '/'), convert to absolute.
var apiBase = apiSettings.EsaApiBaseUrl ?? string.Empty;
if (!string.IsNullOrWhiteSpace(apiBase) && !Uri.IsWellFormedUriString(apiBase, UriKind.Absolute))
{
    // make relative to host base address
    apiBase = new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBase.TrimStart('/')).ToString();
}

// Configure HttpClient for IFleetService to use the resolved API base
builder.Services.AddHttpClient<IFleetService, FleetService>(client =>
{
    if (!string.IsNullOrWhiteSpace(apiBase) && Uri.IsWellFormedUriString(apiBase, UriKind.Absolute))
    {
        client.BaseAddress = new Uri(apiBase);
    }
    else
    {
        // fallback to same-origin, so calls like "api/fleets/random" target the app host
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    }
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register domain logic
builder.Services.AddScoped<IAnchoragePlanner, AnchoragePlanner>();

await builder.Build().RunAsync();
