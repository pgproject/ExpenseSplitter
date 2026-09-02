using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Expense;

namespace ExpenseSplitter.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; private set; }
        public Guid CategoryId { get; private set; }
        public Guid UserIdAdded { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public Currency Currency { get; private set; }
        public List<ExpenseAllocation> ExpenseAllocations { get; private set; } = new();

        public Expense(Guid userId, string name, Guid categoryId, Currency currency, DateTime? date, ExpenseAllocation? expenseAllocation)
        {
            Id = Guid.NewGuid();
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

            if (date == null)
            {
                Date = DateTime.Now;
            }
            else
            {
                Date = date.Value;
            }

            if (expenseAllocation != null)
            {
                ExpenseAllocations.Add(expenseAllocation);
            }
        }

        public void Update(string? newName, Guid? newCategoryId, DateTime? newDate, Currency newCurrency, ExpenseAllocation? expenseAllocation)
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                Name = newName;
            }

            if (newCategoryId.HasValue)
            {
                if (Guid.Empty == newCategoryId)
                {
                    throw new InvalidExpenseCategoryException();
                }

                CategoryId = newCategoryId.Value;
            }

            if (newDate.HasValue && newDate != Date)
            {
                Date = newDate.Value;
            }
            
            if (Currency != newCurrency)
            {
                if (!Enum.IsDefined(newCurrency))
                {
                    throw new InvalidExpenseCurrencyException();
                }

                Currency = newCurrency;
            }

            if (expenseAllocation != null)
            {
                ExpenseAllocations.Add(expenseAllocation);
            }
        }
    }
}
