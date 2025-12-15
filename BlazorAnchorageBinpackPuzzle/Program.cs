using BlazorAnchorageBinpackPuzzle;
using BlazorAnchorageBinpackPuzzle.Services;
using BlazorAnchorageBinpackPuzzle.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ----------------------------------------------------
// HttpClient
// ----------------------------------------------------
// IMPORTANT:
// BaseAddress must be the app origin.
// IIS reverse proxy will handle /api/* forwarding.
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

// Application services
builder.Services.AddScoped<IFleetService, FleetService>();

// Register domain logic
builder.Services.AddScoped<IAnchoragePlanner, AnchoragePlanner>();

await builder.Build().RunAsync();
