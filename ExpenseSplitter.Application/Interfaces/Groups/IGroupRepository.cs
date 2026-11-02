using ExpenseSplitter.Domain.Entities.Groups;

namespace ExpenseSplitter.Application.Interfaces.Groups
{
    public interface IGroupRepository
    {
        Task<Group?> FindByIdAsync(Guid groupId);
        Task<List<Group>> FindAllForUserAsync(Guid userId);
        Task AddAsync(Group group);
        Task UpdateAsync(Group user);
        Task DeleteAsync(Group user);
        Task SaveChangesAsync();
    }
}
