using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Expense;

namespace ExpenseSplitter.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; private set; }
        public Guid CategoryId { get; private set; }
        public Guid UserIdAdded { get; private set; }
        public decimal Amount { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public Currency Currency { get; private set; }
        public List<ExpenseAllocation> ExpenseAllocations { get; private set; } = new();

        private Expense() 
        {
        }
        public Expense(Guid expenseId, string name, Guid userId, Guid categoryId, Currency currency, decimal amount, DateTime? date)
        {
            if (expenseId == Guid.Empty)
            {
                throw new InvalidExpenseIdException();
            }
            Id = expenseId;

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidExpenseNameException();
            }
            Name = name;

            if (Guid.Empty == userId)
            {
                throw new InvalidExpenseAddedByUserException();
            }
            UserIdAdded = userId;

            if (Guid.Empty == categoryId)
            {
                throw new InvalidExpenseCategoryException();
            }
            else
            {
                CategoryId = categoryId;
            }

            if (!Enum.IsDefined(currency))
            {
                throw new InvalidExpenseCurrencyException();
            }

            Currency = currency;

            if (amount <= 0)
            {
                throw new InvalidExpenseAmountException();
            }
            Amount = amount;

            var expenseDate = date ?? DateTime.Now;

            ValidateDate(expenseDate);
            Date = expenseDate;
        }

        public void Update(string? newName, decimal? newAmount, Guid? newCategoryId, DateTime? newDate, Currency newCurrency)
        {
            if (!Enum.IsDefined(newCurrency))
            {
                throw new InvalidExpenseCurrencyException();
            }

            if (newAmount.HasValue && newAmount.Value <= 0)
            {
                throw new InvalidExpenseAmountException();
            }

            if (newCategoryId.HasValue && newCategoryId.Value == Guid.Empty)
            {
                throw new InvalidExpenseCategoryException();
            }

            if (newDate.HasValue)
            {
                ValidateDate(newDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(newName))
            {
                Name = newName;
            }

            if (newAmount.HasValue)
            {
                Amount = newAmount.Value;
            }

            if (newCategoryId.HasValue)
            {
                CategoryId = newCategoryId.Value;
            }

            if (newDate.HasValue)
            {
                Date = newDate.Value;
            }

            Currency = newCurrency;
        }

        public void AddAllocation(ExpenseAllocation allocation)
        {
            if (allocation == null ||
                allocation.ExpenseId != Id ||
                ExpenseAllocations.Any(x => x.GroupId == allocation.GroupId))
            {
                throw new InvalidExpenseAllocationException();
            }

            ExpenseAllocations.Add(allocation);
        }

        private static void ValidateDate(DateTime date)
        {
            if (date.Date > DateTime.Now.Date)
            {
                throw new InvalidExpenseDateException();
            }
        }
    }
}