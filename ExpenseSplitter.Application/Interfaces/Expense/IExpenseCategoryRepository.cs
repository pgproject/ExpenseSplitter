using ExpenseSplitter.Domain.Entities.ExpesneCategory;

namespace ExpenseSplitter.Application.Interfaces.Expense
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory?> FindByIdAsync(Guid categoryId);
        Task<ExpenseCategory?> FindByNameAsync(string name, Guid userId);
        Task<ExpenseCategory?> FindPublicByNameAsync(string name);
        Task<List<ExpenseCategory>> FindAllForUser(Guid userId);
        Task AddAsync(ExpenseCategory group);
        Task UpdateAsync(ExpenseCategory user);
        Task DeleteAsync(ExpenseCategory user);
        Task SaveChangesAsync();
    }
}
