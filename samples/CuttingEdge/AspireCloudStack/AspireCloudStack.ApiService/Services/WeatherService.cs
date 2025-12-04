using AspireCloudStack.ApiService.Models;

namespace AspireCloudStack.ApiService.Services;

public class WeatherService : IWeatherService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherService> _logger;

    public WeatherService(ILogger<WeatherService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days = 5)
    {
        _logger.LogInformation("Generating weather forecast for {Days} days", days);

        var forecasts = Enumerable.Range(1, days).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .AsEnumerable();

        return Task.FromResult(forecasts);
    }
}
