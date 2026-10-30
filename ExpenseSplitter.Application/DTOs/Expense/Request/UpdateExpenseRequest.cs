using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Application.DTOs.Expense.Request
{
    public record UpdateExpenseRequest(Guid id,
        string newName,
        decimal amount,
        Guid categoryId, 
        Currency currency, 
        DateTime newDate,
        CreateExpenseAllocationRequest? expenseAllocation)
    {
    }
}