using ExpenseSplitter.Domain.Entities;

namespace ExpenseSplitter.Application.Interfaces.Users
{
    public interface IUserRepository
    {
        Task<User?> FindByIdAsync(Guid userId);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();
    }
}