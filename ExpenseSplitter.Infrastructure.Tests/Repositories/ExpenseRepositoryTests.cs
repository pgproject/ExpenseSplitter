using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Tests.Repositories
{
    public class ExpenseRepositoryTests
    {
        private static DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(TestDatabaseConfiguration.ConnectionString)
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_SaveExpenseWithAllocations()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var participantId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Test expense",
                userId,
                categoryId,
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            expense.AddAllocation(new ExpenseAllocation(
                expenseId,
                groupId,
                new List<Guid> { participantId }));

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);

                await repository.AddAsync(expense);
                await repository.SaveChangesAsync();
            }

            // Assert
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var savedExpense = await context.Expenses
                    .Include(x => x.ExpenseAllocations)
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                Assert.NotNull(savedExpense);
                Assert.Equal("Test expense", savedExpense.Name);
                Assert.Equal(100m, savedExpense.Amount);

                var allocation = Assert.Single(savedExpense.ExpenseAllocations);

                Assert.Equal(groupId, allocation.GroupId);
                Assert.Equal(expenseId, allocation.ExpenseId);
                Assert.Contains(participantId, allocation.ParticipantUserIds);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var savedExpense = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                if (savedExpense is not null)
                {
                    context.Expenses.Remove(savedExpense);
                    await context.SaveChangesAsync();
                }
            }
        }

        [Fact]
        public async Task FindByIdAsync_Should_ReturnExpenseWithAllocations()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var participantId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Test expense",
                userId,
                categoryId,
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            expense.AddAllocation(new ExpenseAllocation(
                expenseId,
                groupId,
                new List<Guid> { participantId }));

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.Add(expense);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.FindByIdAsync(expenseId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expenseId, result.Id);

                var allocation = Assert.Single(result.ExpenseAllocations);

                Assert.Equal(groupId, allocation.GroupId);
                Assert.Equal(expenseId, allocation.ExpenseId);
                Assert.Contains(participantId, allocation.ParticipantUserIds);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var savedExpense = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                if (savedExpense is not null)
                {
                    context.Expenses.Remove(savedExpense);
                    await context.SaveChangesAsync();
                }
            }
        }

        [Fact]
        public async Task FindByIdAsync_Should_ReturnNull_WhenExpenseDoesNotExist()
        {
            // Arrange
            var expenseId = Guid.NewGuid();

            // Act
            await using var context = new AppDbContext(CreateOptions());
            var repository = new ExpenseRepository(context);

            var result = await repository.FindByIdAsync(expenseId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAllAddedByUser_Should_ReturnOnlyExpensesAddedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var expense1 = new Expense(
                Guid.NewGuid(), "Expense 1", userId, categoryId,
                Currency.PLN, 100m, DateTime.UtcNow);

            var expense2 = new Expense(
                Guid.NewGuid(), "Expense 2", userId, categoryId,
                Currency.PLN, 200m, DateTime.UtcNow);

            var expense3 = new Expense(
                Guid.NewGuid(), "Expense 3", anotherUserId, categoryId,
                Currency.PLN, 300m, DateTime.UtcNow);

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.AddRange(expense1, expense2, expense3);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);

                var result = await repository.FindAllAddedByUser(userId);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.All(result, expense => Assert.Equal(userId, expense.UserIdAdded));
                Assert.Contains(result, expense => expense.Id == expense1.Id);
                Assert.Contains(result, expense => expense.Id == expense2.Id);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.RemoveRange(
                    await context.Expenses
                        .Where(x => x.Id == expense1.Id ||
                                    x.Id == expense2.Id ||
                                    x.Id == expense3.Id)
                        .ToListAsync());

                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task FindAllForGroup_Should_ReturnOnlyExpensesAssignedToGroup()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var anotherGroupId = Guid.NewGuid();

            var expense1 = new Expense(
                Guid.NewGuid(), "Expense 1", userId, categoryId,
                Currency.PLN, 100m, DateTime.UtcNow);

            expense1.AddAllocation(new ExpenseAllocation(
                expense1.Id, groupId, new List<Guid> { userId }));

            var expense2 = new Expense(
                Guid.NewGuid(), "Expense 2", userId, categoryId,
                Currency.PLN, 200m, DateTime.UtcNow);

            expense2.AddAllocation(new ExpenseAllocation(
                expense2.Id, anotherGroupId, new List<Guid> { userId }));

            var expense3 = new Expense(
                Guid.NewGuid(), "Expense 3", userId, categoryId,
                Currency.PLN, 300m, DateTime.UtcNow);

            expense3.AddAllocation(new ExpenseAllocation(
                expense3.Id, groupId, new List<Guid> { userId }));

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.AddRange(expense1, expense2, expense3);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.FindAllForGroup(groupId);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(result, x => x.Id == expense1.Id);
                Assert.Contains(result, x => x.Id == expense3.Id);
                Assert.DoesNotContain(result, x => x.Id == expense2.Id);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var expenses = await context.Expenses
                    .Where(x => x.Id == expense1.Id ||
                                x.Id == expense2.Id ||
                                x.Id == expense3.Id)
                    .ToListAsync();

                context.Expenses.RemoveRange(expenses);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task FindByNameAsync_Should_ReturnExpensesContainingName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var searchTerm = $"Groceries-{Guid.NewGuid():N}";

            var expense1 = new Expense(
                Guid.NewGuid(), $"{searchTerm} shopping", userId, categoryId,
                Currency.PLN, 100m, DateTime.UtcNow);

            var expense2 = new Expense(
                Guid.NewGuid(), $"Weekly {searchTerm}", userId, categoryId,
                Currency.PLN, 200m, DateTime.UtcNow);

            var expense3 = new Expense(
                Guid.NewGuid(), "Cinema tickets", userId, categoryId,
                Currency.PLN, 50m, DateTime.UtcNow);

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.AddRange(expense1, expense2, expense3);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.FindByNameAsync(searchTerm);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(result, x => x.Id == expense1.Id);
                Assert.Contains(result, x => x.Id == expense2.Id);
                Assert.DoesNotContain(result, x => x.Id == expense3.Id);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var expenses = await context.Expenses
                    .Where(x => x.Id == expense1.Id ||
                                x.Id == expense2.Id ||
                                x.Id == expense3.Id)
                    .ToListAsync();

                context.Expenses.RemoveRange(expenses);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateExpense()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Original expense",
                userId,
                categoryId,
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.Add(expense);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);

                var expenseToUpdate = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                Assert.NotNull(expenseToUpdate);

                expenseToUpdate.Update(
                    "Updated expense",
                    250m,
                    null,
                    null,
                    Currency.PLN);

                await repository.UpdateAsync(expenseToUpdate);
                await repository.SaveChangesAsync();
            }

            // Assert
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var updatedExpense = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                Assert.NotNull(updatedExpense);
                Assert.Equal("Updated expense", updatedExpense.Name);
                Assert.Equal(250m, updatedExpense.Amount);
            }

            // Cleanup
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var savedExpense = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                if (savedExpense is not null)
                {
                    context.Expenses.Remove(savedExpense);
                    await context.SaveChangesAsync();
                }
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_RemoveExpense()
        {
            // Arrange
            var expenseId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Expense to delete",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            await using (var context = new AppDbContext(CreateOptions()))
            {
                context.Expenses.Add(expense);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var repository = new ExpenseRepository(context);

                var expenseToDelete = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                Assert.NotNull(expenseToDelete);

                await repository.DeleteAsync(expenseToDelete);
                await repository.SaveChangesAsync();
            }

            // Assert
            await using (var context = new AppDbContext(CreateOptions()))
            {
                var deletedExpense = await context.Expenses
                    .FirstOrDefaultAsync(x => x.Id == expenseId);

                Assert.Null(deletedExpense);
            }
        }
    }
}
