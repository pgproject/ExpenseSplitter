using ExpenseSplitter.Domain.Entities;

namespace ExpenseSplitter.Application.Interfaces;

public interface IExpenseRepository
{
    Task<Expense> FindByIdAsync(Guid id);
    Task<List<Expense>> FindByNameAsync(string name);
    Task<List<Expense>> FindAllForGroup(Guid groupId);
    Task<List<Expense>> FindAllAddedByUser(Guid userId);
    Task AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
    Task SaveChangesAsync();
}
