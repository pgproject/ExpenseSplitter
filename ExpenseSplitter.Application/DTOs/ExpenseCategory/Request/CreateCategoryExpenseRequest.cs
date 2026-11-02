namespace ExpenseSplitter.Application.DTOs.ExpenseCategory.Request
{
    public record CreateCategoryExpenseRequest(Guid categoryId, string name)
    {
    }
}
