using ExpenseSplitter.Domain.Entities;

namespace ExpenseSplitter.Application.Interfaces
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory?> FindByIdAsync(Guid categoryId);
        Task<ExpenseCategory?> FindByNameAsync(string name, Guid userId);
        Task<ExpenseCategory?> FindPublicByNameAsync(string name);
        Task<List<ExpenseCategory>> FindAllForUser(Guid userId);
        Task AddAsync(ExpenseCategory category);
        Task UpdateAsync(ExpenseCategory category);
        Task DeleteAsync(ExpenseCategory category);
        Task SaveChangesAsync();
    }
}
