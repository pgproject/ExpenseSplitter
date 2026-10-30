namespace ExpenseSplitter.Application.DTOs.Expense.Request
{
    public record CreateExpenseAllocationRequest(Guid groupId,
        List<Guid> participantUserIds)
    {
    }
}
