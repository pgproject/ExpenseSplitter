using ExpenseSplitter.Application.Interfaces;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Entities.User;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _dbContext;

        public ExpenseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Expense expense)
        {
            await _dbContext.Expenses.AddAsync(expense);
        }

        public Task DeleteAsync(Expense expense)
        {
            _dbContext.Expenses.Remove(expense);
            return Task.CompletedTask;
        }

        public Task<List<Expense>> FindAllAddedByUser(Guid userId)
        {
            return _dbContext.Expenses.Where(x => x.UserIdAdded == userId).ToListAsync();
        }

        public Task<List<Expense>> FindAllForGroup(Guid groupId)
        {
            return _dbContext.Expenses.Where(x => x.UserIdAdded == userId).ToListAsync();

        }

        public Task<Expense> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Expense>> FindByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Expense expense)
        {
            throw new NotImplementedException();
        }
    }
}
