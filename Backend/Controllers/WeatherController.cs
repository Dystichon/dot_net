using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather()
    {
        var (temperature, condition, place, windSpeed, windDirection) = await _weatherService.GetWeatherAsync();
        
        return Ok(new 
        {
            temperature,
            place,
            condition,
            windSpeed,
            windDirection
        });
    }
}
