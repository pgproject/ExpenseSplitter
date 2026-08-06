using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Infrastructure.Data;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        private GroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }

        public async Task DeleteAsync(Group user)
        {
        }

        public Task<Group?> FindByIdAsync(Guid groupId)
        {
            return _context.Groups.FindAsync(groupId).AsTask();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(Group user)
        {
            throw new NotImplementedException();
        }
    }
}
