using ExpenseSplitter.Application.Interfaces.Expense;
using ExpenseSplitter.Domain.Entities.ExpesneCategory;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly AppDbContext _context;

        public ExpenseCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExpenseCategory category)
        {
            await _context.ExpenseCategory.AddAsync(category);
        }

        public Task DeleteAsync(ExpenseCategory category)
        {
            _context.Remove(category);
            return Task.CompletedTask;
        }

        public Task<List<ExpenseCategory>> FindAllForUser(Guid userId)
        {
            return _context.ExpenseCategory.Where(x => x.UserId == userId || x.UserId == null).ToListAsync();
        }

        public Task<ExpenseCategory?> FindByIdAsync(Guid categoryId)
        {
            return _context.ExpenseCategory.FirstOrDefaultAsync(x => x.Id == categoryId);
        }

        public Task<ExpenseCategory?> FindByNameAsync(string name, Guid userId)
        {
            return _context.ExpenseCategory.FirstOrDefaultAsync(x => x.Name == name && x.UserId == userId);
        }

        public Task<ExpenseCategory?> FindPublicByNameAsync(string name)
        {
            return _context.ExpenseCategory.FirstOrDefaultAsync(x => x.Name == name && x.UserId == null);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(ExpenseCategory category)
        {
            return Task.CompletedTask;
        }
    }
}
