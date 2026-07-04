using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    [HttpGet]
    public IActionResult GetWeather()
    {
        // Mock weather data - in a real app, you'd call a weather API
        // For demo purposes, return some sample data
        var random = new Random();
        var temperature = random.Next(-10, 40); // Temperature in Celsius
        var places = new[] { "Amsterdam", "Rotterdam", "Utrecht", "Eindhoven", "Groningen" };
        var place = places[random.Next(places.Length)];
        
        return Ok(new 
        {
            temperature,
            place
        });
    }
}
