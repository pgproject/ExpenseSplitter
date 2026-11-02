namespace ExpenseSplitter.Application.DTOs.Expense.Response
{
    public record ExpenseAllocationResponse(Guid GroupId,
    List<Guid> ParticipantUserIds)
    {
    }
}
