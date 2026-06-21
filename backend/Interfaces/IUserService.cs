using backend.DTOs;

namespace backend.Interfaces;

public interface IUserService
{
    Task<(UserDto? User, string? ErrorField, string? ErrorMessage, int StatusCode)> GetUserByIdAsync(Guid userId, string requestingUserId);
    Task<(UserDto? User, string? ErrorField, string? ErrorMessage, int StatusCode)> UpdateUserAsync(Guid userId, UpdateUserDto dto, string requestingUserId);
}