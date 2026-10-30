using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Application.DTOs.Expense.Request
{
    public record CreateExpenseRequest(string name, 
        decimal amount, 
        Guid categoryId, 
        Currency? currency, 
        DateTime? date,
        CreateExpenseAllocationRequest? allocation)
    {
    }
}
