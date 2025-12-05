var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisClient("cache");

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Text("""
    <!DOCTYPE html>
    <html>
    <head><title>Aspire Cloud Stack</title></head>
    <body>
        <h1>🚀 .NET Aspire Cloud Stack</h1>
        <p>Multi-service application with service discovery and observability.</p>
        <ul>
            <li><a href="/weather">Weather Forecast</a></li>
            <li><a href="/health">Health Check</a></li>
        </ul>
    </body>
    </html>
    """, "text/html"));

app.MapGet("/weather", async (WeatherApiClient client) =>
{
    var weather = await client.GetWeatherAsync();
    return Results.Ok(weather);
});

app.Run();

public class WeatherApiClient
{
    private readonly HttpClient _httpClient;

    public WeatherApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]> GetWeatherAsync()
    {
        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast") ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
