using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities.User;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public Task DeleteAsync(User user)
        {
            _dbContext.Remove(user);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return _dbContext.Users.AnyAsync(x => x.Email == email);
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            return _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<User?> FindByIdAsync(Guid userId)
        {
            return _dbContext.Users.FindAsync(userId).AsTask();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
