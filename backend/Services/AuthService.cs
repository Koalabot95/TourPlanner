using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;

namespace backend.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private static readonly ILog _log = LogManager.GetLogger(typeof(AuthService));

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
}