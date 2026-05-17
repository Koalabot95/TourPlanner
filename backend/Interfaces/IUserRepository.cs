namespace backend.Interfaces;

public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<Guid> CreateUserAsync(Models.User user);
}