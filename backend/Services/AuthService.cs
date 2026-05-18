using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private static readonly ILog _log = LogManager.GetLogger(typeof(AuthService));

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<(bool success, string field, string message, Guid userId)> RegisterAsync(RegisterDto dto)
    {
        _log.Info($"Registration attempt for username: {dto.Username}");

        if (await _userRepository.UsernameExistsAsync(dto.Username))
        {
            _log.Warn($"Registration failed - username already taken: {dto.Username}");
            return (false, "username", "Username is already taken.", Guid.Empty);
        }

        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            _log.Warn($"Registration failed - email already taken: {dto.Email}");
            return (false, "email", "Email is already in use.", Guid.Empty);
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepository.CreateUserAsync(user);
        _log.Info($"Registration successful for username: {dto.Username}, ID: {id}");
        return (true, "", "", id);
    }

    public async Task<(bool success, string message, string token)> LoginAsync(LoginDto dto)
    {
        _log.Info($"Login attempt for username: {dto.Username}");

        // Check if user exists
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null)
        {
            _log.Warn($"Login failed - username not found: {dto.Username}");
            return (false, "Invalid username or password.", "");
        }

        // Verify password with BCrypt
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _log.Warn($"Login failed - wrong password for username: {dto.Username}");
            return (false, "Invalid username or password.", "");
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        _log.Info($"Login successful for username: {dto.Username}");
        return (true, "", token);
    }

    private string GenerateJwtToken(User user)
    {
        // Read secret from appsettings.json
        var secret = _configuration["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // JWT payload: userId, username, email, expiry
        var claims = new[]
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim("username", user.Username),
            new Claim("email", user.Email),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // expires after 24 hours
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}