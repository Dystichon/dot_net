using System.Security.Claims;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Backend.Services;

public class UserService
{
    private readonly List<User> _users = new();
    private readonly IConfiguration _configuration;

    public UserService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Add a default user for testing
        _users.Add(new User { Username = "test", Password = "test123" });
    }

    private string JwtKey => _configuration["Jwt:Key"] ?? "MySuperSecretKeyThatIsAtLeast32CharactersLong12345";
    private string JwtIssuer => _configuration["Jwt:Issuer"] ?? "https://localhost";

    public AuthResponse? Authenticate(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
        if (user == null) return null;

        var token = GenerateJwtToken(user.Username);
        return new AuthResponse { Token = token, Username = user.Username };
    }

    public AuthResponse? Register(RegisterRequest request)
    {
        if (_users.Any(u => u.Username == request.Username))
            return null; // Username already exists

        var user = new User { Username = request.Username, Password = request.Password };
        _users.Add(user);

        var token = GenerateJwtToken(user.Username);
        return new AuthResponse { Token = token, Username = user.Username };
    }

    public string? GetUsernameFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(JwtKey);
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = JwtIssuer,
                ValidAudience = JwtIssuer,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(JwtKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = JwtIssuer,
            Audience = JwtIssuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
