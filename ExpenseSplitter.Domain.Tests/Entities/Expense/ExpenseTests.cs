using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Expense;
using ExpenseEntity = ExpenseSplitter.Domain.Entities.Expense;

namespace ExpenseSplitter.Domain.Tests.Entities.Expense
{
    public class ExpenseTests
    {
        private static ExpenseEntity CreateValidExpense()
        {
            return new ExpenseEntity(
                Guid.NewGuid(),
                "Zakupy",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                new DateTime(2026, 1, 15));
        }

        [Fact]
        public void Constructor_Should_CreateExpense_WhenDataIsValid()
        {
            var expense = CreateValidExpense();

            Assert.Equal("Zakupy", expense.Name);
            Assert.Equal(100m, expense.Amount);
            Assert.Equal(Currency.PLN, expense.Currency);
            Assert.NotEqual(Guid.Empty, expense.Id);
            Assert.Empty(expense.ExpenseAllocations);
        }

        [Fact]
        public void Constructor_Should_Throw_WhenExpenseIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseIdException>(() =>
                new ExpenseEntity(
                    Guid.Empty,
                    "Zakupy",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Currency.PLN,
                    100m,
                    null));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenNameIsEmpty()
        {
            Assert.Throws<InvalidExpenseNameException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    " ",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Currency.PLN,
                    100m,
                    null));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenUserIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseAddedByUserException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    "Zakupy",
                    Guid.Empty,
                    Guid.NewGuid(),
                    Currency.PLN,
                    100m,
                    null));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenCategoryIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseCategoryException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    "Zakupy",
                    Guid.NewGuid(),
                    Guid.Empty,
                    Currency.PLN,
                    100m,
                    null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Should_Throw_WhenAmountIsNotPositive(decimal amount)
        {
            Assert.Throws<InvalidExpenseAmountException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    "Zakupy",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Currency.PLN,
                    amount,
                    null));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenCurrencyIsInvalid()
        {
            Assert.Throws<InvalidExpenseCurrencyException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    "Zakupy",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    (Currency)999,
                    100m,
                    null));
        }

        [Fact]
        public void Constructor_Should_UseProvidedDate()
        {
            var date = new DateTime(2026, 1, 15);

            var expense = new ExpenseEntity(
                Guid.NewGuid(),
                "Zakupy",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                date);

            Assert.Equal(date, expense.Date);
        }

        [Fact]
        public void Constructor_Should_SetCurrentDate_WhenDateIsNull()
        {
            var beforeCreation = DateTime.Now;

            var expense = new ExpenseEntity(
                Guid.NewGuid(),
                "Zakupy",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                null);

            var afterCreation = DateTime.Now;

            Assert.InRange(expense.Date, beforeCreation, afterCreation);
        }

        [Fact]
        public void AddAllocation_Should_AddAllocation_WhenValid()
        {
            var expense = CreateValidExpense();
            var allocation = new ExpenseAllocation(
                expense.Id,
                Guid.NewGuid(),
                new List<Guid> { Guid.NewGuid() });

            expense.AddAllocation(allocation);

            Assert.Single(expense.ExpenseAllocations);
            Assert.Same(allocation, expense.ExpenseAllocations[0]);
        }

        [Fact]
        public void AddAllocation_Should_Throw_WhenExpenseIdDoesNotMatch()
        {
            var expense = CreateValidExpense();
            var allocation = new ExpenseAllocation(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<Guid> { Guid.NewGuid() });

            Assert.Throws<InvalidExpenseAllocationException>(() =>
                expense.AddAllocation(allocation));
        }

        [Fact]
        public void AddAllocation_Should_Throw_WhenGroupAlreadyExists()
        {
            var expense = CreateValidExpense();
            var groupId = Guid.NewGuid();

            expense.AddAllocation(new ExpenseAllocation(
                expense.Id,
                groupId,
                new List<Guid> { Guid.NewGuid() }));

            Assert.Throws<InvalidExpenseAllocationException>(() =>
                expense.AddAllocation(new ExpenseAllocation(
                    expense.Id,
                    groupId,
                    new List<Guid> { Guid.NewGuid() })));

            Assert.Single(expense.ExpenseAllocations);
        }

        [Fact]
        public void Update_Should_UpdateAmount_WithoutAddingAllocations()
        {
            var expense = CreateValidExpense();

            expense.AddAllocation(new ExpenseAllocation(
                expense.Id,
                Guid.NewGuid(),
                new List<Guid> { Guid.NewGuid() }));

            expense.Update(
                newName: null,
                newAmount: 250m,
                newCategoryId: null,
                newDate: null,
                newCurrency: Currency.PLN);

            Assert.Equal(250m, expense.Amount);
            Assert.Single(expense.ExpenseAllocations);
        }

        [Fact]
        public void Update_Should_Throw_WhenAmountIsNotPositive()
        {
            var expense = CreateValidExpense();

            Assert.Throws<InvalidExpenseAmountException>(() =>
                expense.Update(
                    null,
                    0m,
                    null,
                    null,
                    Currency.PLN));

            Assert.Equal(100m, expense.Amount);
        }

        [Fact]
        public void Update_Should_Throw_WhenCategoryIdIsEmpty()
        {
            var expense = CreateValidExpense();

            Assert.Throws<InvalidExpenseCategoryException>(() =>
                expense.Update(
                    null,
                    null,
                    Guid.Empty,
                    null,
                    Currency.PLN));
        }

        [Fact]
        public void Update_Should_Throw_WhenCurrencyIsInvalid()
        {
            var expense = CreateValidExpense();

            Assert.Throws<InvalidExpenseCurrencyException>(() =>
                expense.Update(
                    null,
                    null,
                    null,
                    null,
                    (Currency)999));
        }

        [Fact]
        public void Constructor_Should_Throw_When_DateIsInFuture()
        {
            var futureDate = DateTime.Now.Date.AddDays(1);

            Assert.Throws<InvalidExpenseDateException>(() =>
                new ExpenseEntity(
                    Guid.NewGuid(),
                    "Dinner",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Currency.PLN,
                    100m,
                    futureDate));
        }

        [Fact]
        public void Update_Should_Throw_When_NewDateIsInFuture()
        {
            var originalDate = DateTime.Now.Date.AddDays(-1);

            var expense = new ExpenseEntity(
                Guid.NewGuid(),
                "Dinner",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Currency.PLN,
                100m,
                originalDate);

            var futureDate = DateTime.Now.Date.AddDays(1);

            Assert.Throws<InvalidExpenseDateException>(() =>
                expense.Update(
                    "Updated dinner",
                    150m,
                    null,
                    futureDate,
                    Currency.PLN));

            Assert.Equal(originalDate, expense.Date);
        }


        [Fact]
        public void Update_Should_NotModifyExpense_WhenDateIsInFuture()
        {
            var expense = CreateValidExpense();

            var originalName = expense.Name;
            var originalAmount = expense.Amount;
            var originalCategoryId = expense.CategoryId;
            var originalDate = expense.Date;
            var originalCurrency = expense.Currency;

            Assert.Throws<InvalidExpenseDateException>(() =>
                expense.Update(
                    "Updated name",
                    200m,
                    Guid.NewGuid(),
                    DateTime.Now.Date.AddDays(1),
                    Currency.EUR));

            Assert.Equal(originalName, expense.Name);
            Assert.Equal(originalAmount, expense.Amount);
            Assert.Equal(originalCategoryId, expense.CategoryId);
            Assert.Equal(originalDate, expense.Date);
            Assert.Equal(originalCurrency, expense.Currency);
        }

        [Fact]
        public void Update_Should_NotModifyExpense_WhenAmountIsInvalid()
        {
            var expense = CreateValidExpense();

            var originalName = expense.Name;
            var originalAmount = expense.Amount;
            var originalCategoryId = expense.CategoryId;

            Assert.Throws<InvalidExpenseAmountException>(() =>
                expense.Update(
                    "Updated name",
                    0m,
                    Guid.NewGuid(),
                    DateTime.Now.Date,
                    Currency.EUR));

            Assert.Equal(originalName, expense.Name);
            Assert.Equal(originalAmount, expense.Amount);
            Assert.Equal(originalCategoryId, expense.CategoryId);
        }

    }
}
