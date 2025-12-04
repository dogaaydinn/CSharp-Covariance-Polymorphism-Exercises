using AspireCloudStack.ApiService.Models;

namespace AspireCloudStack.ApiService.Services;

public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>> GetForecastAsync(int days = 5);
}
