using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "MySuperSecretKeyThatIsAtLeast32CharactersLong12345";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://localhost";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// In-memory user store (for demo purposes)
builder.Services.AddSingleton<UserService>();

// Weather service with HttpClient for Open-Meteo API
builder.Services.AddHttpClient<WeatherService>();

// Configure the app to listen on port 8080
builder.WebHost.UseUrls("http://*:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.UseHttpsRedirection(); // uncommented for testing
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
