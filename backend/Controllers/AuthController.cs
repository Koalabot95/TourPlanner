using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, field, message, userId) = await _authService.RegisterAsync(dto);

        if (!success)
            return Conflict(new { field, message });

        return StatusCode(201, new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (success, message, token) = await _authService.LoginAsync(dto);

        if (!success)
            return Unauthorized(new { field = "credentials", message });

        return Ok(new { token });
    }
}