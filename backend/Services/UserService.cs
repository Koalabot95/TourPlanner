using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using log4net;

namespace backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(UserService));

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(UserDto? User, string? ErrorField, string? ErrorMessage, int StatusCode)> GetUserByIdAsync(Guid userId, string requestingUserId)
    {
        _logger.Info($"Attempting to retrieve user {userId} by user {requestingUserId}");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.Warn($"User {userId} not found");
            return (null, "userId", "User does not exist.", 404);
        }

        if (user.UserId.ToString() != requestingUserId)
        {
            _logger.Warn($"User {requestingUserId} attempted to access another user's profile {userId}");
            return (null, "auth", "No permission to access this user's profile.", 403);
        }

        _logger.Info($"User profile {userId} retrieved successfully");
        return (MapToDto(user), null, null, 200);
    }

    public async Task<(UserDto? User, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateUserAsync(Guid userId, UpdateUserDto dto, string requestingUserId)
    {
        _logger.Info($"Attempting to update user {userId} by user {requestingUserId}");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.Warn($"User {userId} not found for update");
            return (null, "userId", "User does not exist.", 404);
        }

        if (user.UserId.ToString() != requestingUserId)
        {
            _logger.Warn($"User {requestingUserId} attempted to update another user {userId}");
            return (null, "auth", "No permission to update this user's profile.", 403);
        }

        var validation = ValidateUpdateDto(dto);
        if (!validation.IsValid)
        {
            _logger.Warn($"Validation error updating user {userId}: {validation.Message}");
            return (null, validation.Field, validation.Message, 400);
        }

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Bio = dto.Bio ?? user.Bio;

        await _userRepository.UpdateAsync(user);

        _logger.Info($"User {userId} profile updated successfully");
        return (MapToDto(user), null, null, 200);
    }

    private (bool IsValid, string? Field, string? Message) ValidateUpdateDto(UpdateUserDto dto)
    {
        if (dto.FirstName != null && dto.FirstName.Length > 50)
            return (false, "firstName", "First name must not exceed 50 characters.");

        if (dto.LastName != null && dto.LastName.Length > 50)
            return (false, "lastName", "Last name must not exceed 50 characters.");

        return (true, null, null);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt
        };
    }
}