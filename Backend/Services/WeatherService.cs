using System.Net.Http;
using System.Text.Json;

namespace Backend.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    
    // Gouda, Netherlands coordinates
    private const double GoudaLatitude = 52.0150;
    private const double GoudaLongitude = 4.8967;
    
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.open-meteo.com");
    }
    
    public class OpenMeteoResponse
    {
        public CurrentData? Current { get; set; }
    }
    
    public class CurrentData
    {
        public double? Temperature2m { get; set; }
        public int? Weathercode { get; set; }
        public double? Windspeed10m { get; set; }
        public double? Winddirection10m { get; set; }
    }
    
    public async Task<(double Temperature, string Condition, string Place, double WindSpeed, string WindDirection)> GetWeatherAsync()
    {
        // Open-Meteo free API for current weather in Gouda
        var url = $"/v1/forecast?latitude={GoudaLatitude}&longitude={GoudaLongitude}&current=temperature_2m,weathercode,windspeed_10m,winddirection_10m&timezone=Europe/Amsterdam";
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var data = JsonSerializer.Deserialize<OpenMeteoResponse>(json, options);
            
            if (data?.Current?.Temperature2m != null)
            {
                var condition = MapWeatherCode(data.Current.Weathercode);
                var windSpeed = data.Current.Windspeed10m ?? 0;
                var windDirection = MapWindDirection(data.Current.Winddirection10m);
                return (Math.Round(data.Current.Temperature2m.Value, 1), condition, "Gouda", Math.Round(windSpeed, 1), windDirection);
            }
        }
        catch
        {
            // Fallback to mock data if API fails
        }
        
        // Fallback mock data
        var random = new Random();
        var temp = Math.Round(random.NextDouble() * 30 + 5, 1);
        var mockCondition = new[] { "Clear", "Partly cloudy", "Cloudy", "Rain", "Sunny" }[random.Next(5)];
        var mockWindSpeed = Math.Round(random.NextDouble() * 25, 1);
        var mockWindDirection = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }[random.Next(8)];
        return (temp, mockCondition, "Gouda", mockWindSpeed, mockWindDirection);
    }
    
    private static string MapWeatherCode(int? code)
    {
        if (code == null) return "Unknown";
        
        return code switch
        {
            // Clear
            0 => "Clear sky",
            
            // Mainly clear
            1 => "Mainly clear",
            
            // Partly cloudy
            2 => "Partly cloudy",
            
            // Overcast
            3 => "Overcast",
            
            // Fog
            45 => "Fog",
            48 => "Depositing rime fog",
            
            // Drizzle
            51 => "Light drizzle",
            53 => "Moderate drizzle",
            55 => "Heavy drizzle",
            
            // Freezing drizzle
            56 => "Light freezing drizzle",
            57 => "Heavy freezing drizzle",
            
            // Rain
            61 => "Light rain",
            63 => "Moderate rain",
            65 => "Heavy rain",
            
            // Freezing rain
            66 => "Light freezing rain",
            67 => "Heavy freezing rain",
            
            // Snow
            71 => "Light snow",
            73 => "Moderate snow",
            75 => "Heavy snow",
            77 => "Snow grains",
            
            // Rain showers
            80 => "Light rain showers",
            81 => "Moderate rain showers",
            82 => "Heavy rain showers",
            
            // Snow showers
            85 => "Light snow showers",
            86 => "Heavy snow showers",
            
            // Thunderstorm
            95 => "Thunderstorm",
            96 => "Thunderstorm with light hail",
            99 => "Thunderstorm with heavy hail",
            
            _ => "Unknown"
        };
    }
    
    private static string MapWindDirection(double? degrees)
    {
        if (degrees == null) return "Unknown";
        
        // Convert degrees to 16-point compass
        var dirs = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
        var index = (int)((degrees.Value + 11.25) / 22.5) % 16;
        return dirs[index];
    }
}
