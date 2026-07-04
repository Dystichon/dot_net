using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var response = _userService.Authenticate(request);
        if (response == null)
            return Unauthorized(new { message = "Invalid username or password" });
        
        return Ok(response);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var response = _userService.Register(request);
        if (response == null)
            return BadRequest(new { message = "Username already exists" });
        
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;
        if (username == null)
            return Unauthorized();
        
        return Ok(new { username });
    }
}
