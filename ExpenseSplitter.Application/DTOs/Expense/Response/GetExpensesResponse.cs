namespace ExpenseSplitter.Application.DTOs.Expense.Response
{
    public record GetExpensesResponse(Guid ExpenseId,
    string name,
    decimal amount,
    Guid categoryId,
    Guid userIdAdded,
    DateTime date,
    int currency,
    List<ExpenseAllocationResponse> allocations)
    {
    }
}
