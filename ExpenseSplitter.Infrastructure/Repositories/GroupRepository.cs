using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context)
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
            return _context.Groups
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id == groupId);
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
