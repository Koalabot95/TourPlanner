namespace backend.Interfaces;

public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<Guid> CreateUserAsync(Models.User user);
    Task<Models.User?> GetByUsernameAsync(string username);
    Task<Models.User?> GetByIdAsync(Guid userId);
    Task UpdateAsync(Models.User user);
}