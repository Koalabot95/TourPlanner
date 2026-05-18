using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TourPlannerContext _context;

        public UserRepository(TourPlannerContext context)
        {
            _context = context;
        }

        public async Task<bool> UsernameExistsAsync(string username)
            => await _context.Users.AnyAsync(u => u.Username == username);

        public async Task<bool> EmailExistsAsync(string email)
       => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<Guid> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserId;
        }

        public async Task<User?> GetByUsernameAsync(string username)
           => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
