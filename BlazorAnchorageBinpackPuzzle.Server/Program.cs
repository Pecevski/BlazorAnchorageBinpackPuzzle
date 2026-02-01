var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("EsaApi", client =>
{
    client.BaseAddress = new Uri("https://esa.instech.no");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.Map("/fleets/random", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("EsaApi");
    var response = await client.GetAsync("/api/fleets/random");
    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json", statusCode: (int)response.StatusCode);
});

app.MapFallbackToFile("index.html");

app.Run();
