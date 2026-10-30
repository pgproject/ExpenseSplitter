using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.Interfaces;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Application.Services;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Expense;
using Moq;

namespace ExpenseSplitter.Application.Test
{
    public class ExpenseServiceTest
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private readonly Mock<IExpenseRepository> _expenseRepositoryMock = new();
        private readonly Mock<IExpenseCategoryService> _expenseCategoryServiceMock = new();

        private readonly ExpenseService _expenseService;

        public ExpenseServiceTest()
        {
            _expenseService = new ExpenseService(
                _currentUserServiceMock.Object,
                _expenseRepositoryMock.Object,
                _expenseCategoryServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_CreateExpense_When_RequestIsValid()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var request = new CreateExpenseRequest(
                "Dinner",
                150m,
                categoryId,
                Currency.PLN,
                DateTime.UtcNow,
                null);

            var result = await _expenseService.CreateAsync(request);

            Assert.NotEqual(Guid.Empty, result.expenseId);

            _expenseRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Expense>(expense =>
                    expense.Id == result.expenseId &&
                    expense.Name == request.name &&
                    expense.Amount == request.amount &&
                    expense.CategoryId == request.categoryId &&
                    expense.UserIdAdded == userId &&
                    expense.Currency == Currency.PLN)),
                Times.Once);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_AddAllocation_When_RequestContainsAllocation()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var participantIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var allocationRequest = new CreateExpenseAllocationRequest(
                groupId,
                participantIds);

            var request = new CreateExpenseRequest(
                "Dinner",
                150m,
                categoryId,
                Currency.PLN,
                DateTime.UtcNow,
                allocationRequest);

            Expense? savedExpense = null;

            _expenseRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Expense>()))
                .Callback<Expense>(expense => savedExpense = expense)
                .Returns(Task.CompletedTask);

            await _expenseService.CreateAsync(request);

            Assert.NotNull(savedExpense);

            var allocation = Assert.Single(savedExpense.ExpenseAllocations);

            Assert.Equal(groupId, allocation.GroupId);
            Assert.Equal(participantIds, allocation.ParticipantUserIds);
            Assert.Equal(savedExpense.Id, allocation.ExpenseId);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_CreateExpenseWithoutAllocation_When_AllocationIsNull()
        {
            var userId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var request = new CreateExpenseRequest(
                "Dinner",
                150m,
                Guid.NewGuid(),
                Currency.PLN,
                DateTime.UtcNow,
                null);

            Expense? savedExpense = null;

            _expenseRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Expense>()))
                .Callback<Expense>(expense => savedExpense = expense)
                .Returns(Task.CompletedTask);

            await _expenseService.CreateAsync(request);

            Assert.NotNull(savedExpense);
            Assert.Empty(savedExpense.ExpenseAllocations);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_CurrencyIsNull()
        {
            var request = new CreateExpenseRequest(
                "Dinner",
                150m,
                Guid.NewGuid(),
                null,
                DateTime.UtcNow,
                null);

            await Assert.ThrowsAsync<InvalidExpenseCurrencyException>(
                () => _expenseService.CreateAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateExpense_When_RequestIsValid()
        {
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var expense = new Expense(
                expenseId,
                "Old name",
                userId,
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            var newCategoryId = Guid.NewGuid();
            var newDate = DateTime.UtcNow.AddDays(-1);

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new UpdateExpenseRequest(
                expenseId,
                "New name",
                200m,
                newCategoryId,
                Currency.EUR,
                newDate,
                null);

            var result = await _expenseService.UpdateAsync(request);

            Assert.Equal(expenseId, result.Id);
            Assert.Equal("New name", expense.Name);
            Assert.Equal(200m, expense.Amount);
            Assert.Equal(newCategoryId, expense.CategoryId);
            Assert.Equal(Currency.EUR, expense.Currency);
            Assert.Equal(newDate, expense.Date);

            _expenseRepositoryMock.Verify(
                x => x.UpdateAsync(expense),
                Times.Once);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_ExpenseDoesNotExist()
        {
            var expenseId = Guid.NewGuid();

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync((Expense)null!);

            var request = new UpdateExpenseRequest(
                expenseId,
                "Dinner",
                100m,
                Guid.NewGuid(),
                Currency.PLN,
                DateTime.UtcNow,
                null);

            await Assert.ThrowsAsync<ExpenseNotFoundException>(
                () => _expenseService.UpdateAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_AmountIsInvalid()
        {
            var expenseId = Guid.NewGuid();

            var userId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Dinner",
                userId,
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new UpdateExpenseRequest(
                expenseId,
                "Dinner",
                0m,
                Guid.NewGuid(),
                Currency.PLN,
                DateTime.UtcNow,
                null);

            await Assert.ThrowsAsync<InvalidExpenseAmountException>(
                () => _expenseService.UpdateAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_NotAddAllocations_When_RequestContainsAllocation()
        {
            var expenseId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Dinner",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(expense.UserIdAdded);

            var existingAllocation = new ExpenseAllocation(
                expenseId,
                Guid.NewGuid(),
                new List<Guid> { Guid.NewGuid() });

            expense.AddAllocation(existingAllocation);

            var originalAllocationCount = expense.ExpenseAllocations.Count;

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new UpdateExpenseRequest(
                expenseId,
                "Updated dinner",
                150m,
                Guid.NewGuid(),
                Currency.PLN,
                DateTime.UtcNow,
                new CreateExpenseAllocationRequest(
                    Guid.NewGuid(),
                    new List<Guid> { Guid.NewGuid() }));

            await _expenseService.UpdateAsync(request);

            Assert.Equal(
                originalAllocationCount,
                expense.ExpenseAllocations.Count);

            Assert.Same(
                existingAllocation,
                expense.ExpenseAllocations.Single());

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_CurrentUserIsNotExpenseOwner()
        {
            var expenseId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Dinner",
                ownerId,
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new UpdateExpenseRequest(
                expenseId,
                "Updated dinner",
                150m,
                Guid.NewGuid(),
                Currency.PLN,
                DateTime.UtcNow,
                null);

            await Assert.ThrowsAsync<UserIsNotExpenseOwnerException>(
                () => _expenseService.UpdateAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task RemoveAsync_Should_RemoveExpense_When_ExpenseExists()
        {
            var expenseId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Dinner",
                ownerId,
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new RemoveExpenseRequest(expenseId);

            var result = await _expenseService.RemoveAsync(request);

            Assert.Equal(expenseId, result.Id);

            _expenseRepositoryMock.Verify(
                x => x.FindByIdAsync(expenseId),
                Times.Once);

            _expenseRepositoryMock.Verify(
                x => x.DeleteAsync(expense),
                Times.Once);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_Should_Throw_When_ExpenseDoesNotExist()
        {
            var expenseId = Guid.NewGuid();

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync((Expense)null!);

            var request = new RemoveExpenseRequest(expenseId);

            await Assert.ThrowsAsync<ExpenseNotFoundException>(
                () => _expenseService.RemoveAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task RemoveAsync_Should_Throw_When_CurrentUserIsNotExpenseOwner()
        {
            var expenseId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var expense = new Expense(
                expenseId,
                "Dinner",
                ownerId,
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                DateTime.UtcNow);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _expenseRepositoryMock
                .Setup(x => x.FindByIdAsync(expenseId))
                .ReturnsAsync(expense);

            var request = new RemoveExpenseRequest(expenseId);

            await Assert.ThrowsAsync<UserIsNotExpenseOwnerException>(
                () => _expenseService.RemoveAsync(request));

            _expenseRepositoryMock.Verify(
                x => x.FindByIdAsync(expenseId),
                Times.Once);

            _expenseRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Expense>()),
                Times.Never);

            _expenseRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}

