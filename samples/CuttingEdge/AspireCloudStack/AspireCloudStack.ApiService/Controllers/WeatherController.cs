using AspireCloudStack.ApiService.Models;
using AspireCloudStack.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireCloudStack.ApiService.Controllers;

/// <summary>
/// Weather forecast controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(
        IWeatherService weatherService,
        ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// Get weather forecast
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get([FromQuery] int days = 5)
    {
        _logger.LogInformation("Getting weather forecast for {Days} days", days);

        if (days < 1 || days > 30)
        {
            return BadRequest(new { Message = "Days must be between 1 and 30" });
        }

        var forecasts = await _weatherService.GetForecastAsync(days);
        return Ok(forecasts);
    }
}
