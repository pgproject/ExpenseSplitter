using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Application.Interfaces.Users;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task DeleteAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return _context.Users.AnyAsync(x => x.Email == email);
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<User?> FindByIdAsync(Guid userId)
        {
            return _context.Users.FindAsync(userId).AsTask();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
